namespace SWCPaint.Core.Commands;

public interface IUndoableCommand
{
    string Name { get; }
    void Execute();
    void Undo();
}