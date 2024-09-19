using System;
using System.Collections.Generic;
using Godot;


public class NeuralNetwork 
{
	Random random = new Random();
	public List<NeuralNetworkNode> nodes {get; private set;}
	double connectionProbability;
	double mutationProbability = 0.2;
	int input,output;

	///<summary>
	///A simple dynamic topology neurl network
	///</summary>
	///<param name="input">The number of inputs the network can take</param>
	///<param name="output">The number of outputs the network can have in the output layer</param>
	///<param name="connectionProbability">The probility of forming new connections upon mutation</param>
	///<returns>A nreual netowrok with randomised weights</returns>
	public NeuralNetwork(int input, int output, double connectionProbability){
		this.connectionProbability = connectionProbability;
		this.input = input;
		this.output = output;
		nodes = new List<NeuralNetworkNode>();	

		for(int i = 0; i<input;i++){
			nodes.Add(new NeuralNetworkNode(NeuralNetworkNode.tanh));
		}
		for(int i = 0; i<5;i ++){
			AddRandomNode(NeuralNetworkNode.tanh);	
		}
		for(int i = 0; i<output; i++){
			AddRandomNode(NeuralNetworkNode.tanh, input+10);	
		}
	}

	//Adds a random node into the network and handles making the connections
	void AddRandomNode(ActivationFunction function){
		NeuralNetworkNode newNode = new NeuralNetworkNode(function);
		foreach(NeuralNetworkNode node in nodes){
			if(random.NextDouble() < connectionProbability)
				NeuralNetworkNode.connectNodes(node,newNode, random.NextDouble()*2 - 1);
		}
		nodes.Add(newNode);
	}
	

	//Adds a random node into the network and handles making the connections
	void AddRandomNode(ActivationFunction function, float indexLimit){
		NeuralNetworkNode newNode = new NeuralNetworkNode(function);
		int i = 0;
		foreach(NeuralNetworkNode node in nodes){
			if(i > indexLimit) break;
			if(random.NextDouble() < connectionProbability)
				NeuralNetworkNode.connectNodes(node,newNode, random.NextDouble()*2-1);
			i++;
		}
		nodes.Add(newNode);
	}

	///<summary>
	///Runs the network based on a given input and outputs the result
	///</summary>
	///<returns>
	///An array of doubles which represent the output layer
	///</returns>
	///<param name="inputValues">The inputs that the netowork will run</param>
	public double[] Run(double[] inputValues){
		Flush();
		for(int i = 0; i<inputValues.Length;i++){
			nodes[i].AddToSum(inputValues[i]);
		}
		foreach(NeuralNetworkNode node in nodes){
			node.Run();
		}

		double[] result = new double[output];	
		int index = 0;
		for(int i = nodes.Count - output; i<nodes.Count; i++){
			result[index] = nodes[i].currentSum;
		}

		return result;
	}
	
	//fulushes all the values of the network to clean it from lingering values from the previous run
	void Flush(){
		foreach(NeuralNetworkNode node in nodes){
			node.Flush();
		}
	}

	//Sorts the nodes in the neural network so they run in the correct sequence. This is important becuase
	//this is fundamentaly a directional graph and the connections must run in the correct sequence.
	//This works by checking how many outgoing connections each node has starting from the input nodes and places
	//the nodes in a new list in sequence ensuring there are no backwards connections
	public void Sort(){
		List<NeuralNetworkNode> sortedNodes = new List<NeuralNetworkNode>(nodes.Count);
		List<NeuralNetworkNode> unsortedNodes = new List<NeuralNetworkNode>(nodes);
		
		for(int i = 0; unsortedNodes.Count != 0; i++){
			if(i >= unsortedNodes.Count){
				i=0;
			}

			if(unsortedNodes[i].GetNumberOfActiveConnections() == 0){
				sortedNodes.Add(unsortedNodes[i]);
				unsortedNodes[i].SetActiveOutgoing(false);
				unsortedNodes.RemoveAt(i);
			}

		}

		nodes = sortedNodes;
		foreach(NeuralNetworkNode node in nodes){
			node.SetActiveOutgoing(true);
		}
	}


	///<summary>
	///Preforms one of three mutations on the network randomly
	///These mutations can be:
	///Adding a node in between two nodes and connects them,
	///Adds a new connection between two existing nodes,
	///Modies the weights at random
	///And there is a small chance it does nothing
	///</summary>
	public void Mutate(){
	double value = random.NextDouble();
	if(value < 0.15){
			AddNodeInBetween();
		}else if( value <0.3){
			AddNewConnection();
		}else if(value<0.8){
			ModifyWeights();
		}else{}

	}


	void AddNodeInBetween(){
		List<Tuple<int, NeuralNetworkNode>> newNodes = new List<Tuple<int, NeuralNetworkNode>>();
		for(int i = 0; i<nodes.Count; i++){
			NeuralNetworkNode node = nodes[i];
			for(int j = node.incoming.Count-1; j>-1;j--){
				if(random.NextDouble()<mutationProbability){
					NeuralNetworkLink link = node.incoming[j];
					NeuralNetworkNode from = link.from;
					NeuralNetworkNode to = link.to;
					NeuralNetworkNode.disconnectNodes(link);
					NeuralNetworkNode newNode = new(NeuralNetworkNode.tanh);
					NeuralNetworkNode.connectNodes(from,newNode,random.NextDouble()*2-1);
					NeuralNetworkNode.connectNodes(newNode,to,random.NextDouble()*2-1);
					newNodes.Add(new Tuple<int, NeuralNetworkNode>(i-1, newNode));
					return;
				}
			}
		}

		for(int i=0;i<newNodes.Count;i++){
			nodes.Insert(newNodes[i].Item1+i+1,newNodes[i].Item2);
		}
		if(CheckForCycle()){
			GD.Print("Cycle detected while adding node");
		}
	}

			

	bool CheckForCycle(){
		for(int i = 0; i< nodes.Count;i++){
			NeuralNetworkNode node = nodes[i];
			foreach(NeuralNetworkLink link in node.incoming){
				if(nodes.IndexOf(link.from)>=i)
					return true;
			}
		}
		return false;
	}


	void AddNewConnection(){
		try{
		for(int i = 0; i<nodes.Count;i++){
			for(int j = i-1; j>-1; j--){
				if(random.NextDouble()<mutationProbability){
					if(NeuralNetworkNode.areConnected(nodes[i],nodes[j])){continue;}
					NeuralNetworkNode.connectNodes(nodes[j],nodes[i],random.NextDouble()*2-1);
					return;
				}
			}
		}
		}catch(Exception e){
			GD.Print(e);
		}
		if(CheckForCycle()){
			GD.Print("Cycle detected while adding connection");
		}

	}
	
	void ModifyWeights(){
		foreach(NeuralNetworkNode node in nodes){
			foreach(NeuralNetworkLink link in node.outgoing){
				if(random.NextDouble()<mutationProbability){
					link.weight += random.NextDouble()*2-1;
				}
			}
			if(random.NextDouble()<mutationProbability){
				node.bias += random.NextDouble()*2-1;
			}
		}
	}


} 
