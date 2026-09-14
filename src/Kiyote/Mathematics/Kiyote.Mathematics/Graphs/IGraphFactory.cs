namespace Kiyote.Mathematics.Graphs;

public interface IGraphFactory<TNode, TEdge> {

	IGraph<TNode, TEdge> Create();

}
