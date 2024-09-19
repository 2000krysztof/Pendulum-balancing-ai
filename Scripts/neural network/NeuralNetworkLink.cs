///<summary>A link with two nodes with a weight</summary>
public class NeuralNetworkLink{

	public NeuralNetworkNode from {get; private set;}
	public NeuralNetworkNode to {get; private set;}
	public double weight {get; set;}

	public bool isActive {get; set;}

	public NeuralNetworkLink(NeuralNetworkNode from , NeuralNetworkNode to, double weight){
		this.from = from;
		this.to = to;
		this.weight = weight; 
	}
	
	public void Run(double value){
		to.AddToSum(value*weight);
	}




}
