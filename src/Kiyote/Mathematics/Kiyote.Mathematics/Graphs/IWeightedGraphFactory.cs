namespace Kiyote.Mathematics.Graphs;

public interface IWeightedGraphFactory<TNode, TEdge> {

	IWeightedGraph<TNode, TEdge> Create();

}
