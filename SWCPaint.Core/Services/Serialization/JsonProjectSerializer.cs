using System;
using System.Linq;
using System.Text.Json;
using SWCPaint.Core.Dtos;
using SWCPaint.Core.Factories;
using SWCPaint.Core.Interfaces.Serialization;
using SWCPaint.Core.Models;
using SWCPaint.Core.Services.Serialization;

namespace SWCPaint.Core.Services.Serialization;

public class JsonProjectSerializer : IProjectSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly ElementFactory _elementFactory;

    public JsonProjectSerializer(ElementFactory? elementFactory = null)
    {
        _elementFactory = elementFactory ?? new ElementFactory(_options);
    }

    public string Serialize(Project project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));

        var dto = MapToDto(project);
        
        return JsonSerializer.Serialize(dto, _options);
    }

    public Project Deserialize(string projectData)
    {
        if (string.IsNullOrWhiteSpace(projectData)) 
            throw new ArgumentException("Дані проєкту не можуть бути порожніми.", nameof(projectData));

        var dto = JsonSerializer.Deserialize<ProjectDto>(projectData, _options)
                  ?? throw new InvalidOperationException("Помилка: Не вдалося розпарсити JSON файл проєкту.");

        return MapFromDto(dto);
    }


    private ProjectDto MapToDto(Project project)
    {
        var visitor = new ProjectSerializationVisitor();

        return new ProjectDto
        {
            Width = project.Width,
            Height = project.Height,
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
    }

    private Project MapFromDto(ProjectDto dto)
    {
        var project = new Project(dto.Width, dto.Height, "New Project")
        {
            BackgroundColor = ColorMapper.FromDto(dto.BackgroundColor)
        };

        project.ClearLayers();

        foreach (var layerDto in dto.Layers)
        {
            var layer = MapLayerFromDto(layerDto);
            project.AddLayer(layer);
        }

        return project;
    }

    private Layer MapLayerFromDto(LayerDto layerDto)
    {
        var layer = new Layer(layerDto.Name) { IsVisible = layerDto.IsVisible };

        var elements = layerDto.Elements
            .OfType<JsonElement>() 
            .Select(_elementFactory.CreateElement)
            .Where(e => e != null);

        foreach (var element in elements)
        {
            layer.Elements.Add(element!);
        }

        return layer;
    }
}
