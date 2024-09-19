using System.Collections.Generic;
using System;

///<summary>A node which the neural network is composed of. Contains a list of incoming and outgoing connections and a bias</summary>
public class NeuralNetworkNode{
	
	static Random random = new Random();

	public List<NeuralNetworkLink> incoming {get; private set;}
	public List<NeuralNetworkLink> outgoing {get; private set;}

	ActivationFunction activationFunction;
	public static readonly ActivationFunction relu = x => Math.Max(0,x);
	public static readonly ActivationFunction sigmoid = x => 1/(1+Math.Exp(-x));
	public static readonly ActivationFunction tanh = x => Math.Tanh(x);

	
	public double bias {get; set;}
	public double currentSum {get; private set;}

	public NeuralNetworkNode(ActivationFunction function){
		bias = random.NextDouble()*2-1;
		activationFunction = function;	
		incoming = new List<NeuralNetworkLink>();
		outgoing = new List<NeuralNetworkLink>();
	}


	public static void connectNodes(NeuralNetworkNode from, NeuralNetworkNode to, double weight){
		NeuralNetworkLink link = new NeuralNetworkLink(from, to, weight);
		from.outgoing.Add(link);
		to.incoming.Add(link);
	}
	public static void disconnectNodes(NeuralNetworkLink link){
		NeuralNetworkNode from = link.from;
		NeuralNetworkNode to = link.to;
		from.outgoing.Remove(link);
		to.incoming.Remove(link);
	}
	public static bool areConnected(NeuralNetworkNode a, NeuralNetworkNode b){
		foreach(NeuralNetworkLink link in a.outgoing){
			if(link.to == b){return true;}
		}
		foreach(NeuralNetworkLink link in b.outgoing){
			if(link.to == a){return true;}
		}
		return false;
	}

	public static NeuralNetworkLink LinkBetweenTwoNodes(NeuralNetworkNode a, NeuralNetworkNode b){
		foreach(NeuralNetworkLink link in a.incoming){
			if(link.from == b){
				return link;
			}
		}
		foreach(NeuralNetworkLink link in a.outgoing){
			if(link.to == b){
				return link;
			}
		}
		return null;
	}


	public void AddToSum(double value){
		currentSum += value;	
	}

	public void Run(){
		currentSum += bias;
		currentSum = activationFunction(currentSum);
		foreach(NeuralNetworkLink link in outgoing){
			link.Run(currentSum);
		}
	}
	public void Flush(){
	
		currentSum = 0;
	}

	public int GetNumberOfActiveConnections(){
		int activeCount = 0;
		foreach(NeuralNetworkLink link in incoming){
			if(link.isActive){
				activeCount ++;
			}
		}
		return activeCount;
	}

	public void SetActiveOutgoing(bool active){
		foreach(NeuralNetworkLink link in outgoing){
			link.isActive = active;
		}
	}


}


public delegate double ActivationFunction(double input);
