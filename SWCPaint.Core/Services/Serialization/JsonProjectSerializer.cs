using System.Text.Json;
using SWCPaint.Core.Dtos;
using SWCPaint.Core.Factories;
using SWCPaint.Core.Interfaces.Serialization;
using SWCPaint.Core.Models;
using SWCPaint.Core.Services.Serialization;

public class JsonProjectSerializer : IProjectSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };
    private readonly ElementFactory _elementFactory;

    public JsonProjectSerializer()
    {
        _elementFactory = new ElementFactory(_options);
    }

    public string Serialize(Project project)
    {
        var visitor = new ProjectSerializationVisitor();

        var dto = new ProjectDto
        {
            Width = project.Width,
            Height = project.Height,
            // Використовуємо мапер для кольору фону
            BackgroundColor = ColorMapper.ToDto(project.BackgroundColor),
            Layers = project.Layers.Select(l => new LayerDto
            {
                Name = l.Name,
                IsVisible = l.IsVisible,
                Elements = l.Elements.Select(e =>
                {
                    e.Accept(visitor);
                    return visitor.LastSerialized!;
                }).ToList()
            }).ToList()
        };

        return JsonSerializer.Serialize(dto, _options);
    }

    public Project Deserialize(string projectData)
    {
        var dto = JsonSerializer.Deserialize<ProjectDto>(projectData, _options)
                  ?? throw new InvalidOperationException("Не вдалося розпарсити файл проєкту.");

        var project = new Project(dto.Width, dto.Height, "temp")
        {
            BackgroundColor = ColorMapper.FromDto(dto.BackgroundColor)
        };

        project.ClearLayers();

        foreach (var layerDto in dto.Layers)
        {
            var layer = new Layer(layerDto.Name)
            {
                IsVisible = layerDto.IsVisible,
            };

            foreach (var elementObj in layerDto.Elements)
            {
                if (elementObj is JsonElement jsonElement)
                {
                    var element = _elementFactory.CreateElement(jsonElement);
                    if (element != null)
                    {
                        layer.Elements.Add(element);
                    }
                }
            }

            project.AddLayer(layer);
        }

        return project;
    }
}