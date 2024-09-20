using Godot;
using System.Collections.Generic;
using System;
public partial class NeuralNetworkVisualizer : Node2D
{
	public NeuralNetwork neuralNetwork {get; private set;}
	List<DrawableNode> nodes = new List<DrawableNode>();
	[Export] Node2D agent;
	List<List<DrawableNode>> layerdNodes = new List<List<DrawableNode>>();	

	[Export] float layerOffsetMuliplayer, nodeOffsetMultiplayer;

	public override void _Ready()
	{
	}
	public void setTrainerInstance(Trainer trainer){
		trainer.OnSetBest += OnPickBest;
	}

	void OnPickBest(Agent agent){
		nodes = new List<DrawableNode>();
		layerdNodes = new List<List<DrawableNode>>();
		setNeuralNetwork(agent.GetNode<Brain>("Brain:Brain").network);
		QueueRedraw();
	}

	

	public void setNeuralNetwork(NeuralNetwork neuralNetwork){
		this.neuralNetwork = neuralNetwork;
		foreach(NeuralNetworkNode node in neuralNetwork.nodes){
			DrawableNode drawableNode = new DrawableNode(node);
			nodes.Add(drawableNode);
			drawableNode.ConnectNodes(nodes);
		}
		Vector2 offset = new Vector2(20,60);	

		foreach(DrawableNode node in nodes){
			while(layerdNodes.Count-1 < node.depth){
				layerdNodes.Add(new List<DrawableNode>());
			}
			layerdNodes[node.depth].Add(node);
		}

		int index = 0;	
		foreach(List<DrawableNode> layer in layerdNodes){
			Vector2 layerOffset = new Vector2(index, 10/layer.Count)*layerOffsetMuliplayer + offset;
			for(int i = 0; i<layer.Count; i++){
				layer[i].postion = layerOffset + new Vector2(0,i)*nodeOffsetMultiplayer;
			}
			index ++;
		}

	}

	public override void _Draw(){
		Random random = new Random();	
		foreach(DrawableNode node in nodes){
			foreach(Tuple<DrawableNode,Double> connectedNode in node.connectedNodes){
				float r,b;
				if(connectedNode.Item2 < 0){r = 0; b = 1;}
				else{b = 0; r = 1;}
				DrawLine(node.postion, connectedNode.Item1.postion, new Color(r,0.2f,b),Math.Abs((float)connectedNode.Item2*7f));
			}
		}
		foreach(DrawableNode node in nodes){
			DrawCircle(node.postion, 25f, new Color(1,1,1));
			DrawCircle(node.postion, 20f, new Color(1,0.1f,0.1f));
		}
	

	}


	private class DrawableNode{
		public int depth {get; private set;}
		NeuralNetworkNode associatedNode;
		public List<Tuple<DrawableNode,Double>> connectedNodes {get; set;}
		public Vector2 postion {get; set;}

		public DrawableNode(NeuralNetworkNode node){
			depth = GetDepth(node,0);
			associatedNode = node;
			connectedNodes= new List<Tuple<DrawableNode,Double>>();
		}

		int GetDepth(NeuralNetworkNode node, int depth){
			if(depth >10){return 10;}
			if(node.incoming.Count == 0){
				return depth;
			}else{
				int largest = 0;
				foreach(NeuralNetworkLink link in node.incoming){
					int current = GetDepth(link.from, depth+1);
					if(current > largest){largest = current;}
				}
				return largest;
			}
		}

		bool CheckForCycle(NeuralNetworkNode node, List<NeuralNetworkNode> encountered){
			if(node.incoming.Count == 0){
				return false;
			}else{
				foreach(NeuralNetworkLink link in node.incoming){
					if(node == link.from){return true;}
					encountered.Add(link.from);
					CheckForCycle(link.from, encountered);
				}
			}
			return false;
		}
			
		public void ConnectNodes(List<DrawableNode> nodes){
			foreach(DrawableNode node in nodes){
				if(NeuralNetworkNode.areConnected(node.associatedNode, associatedNode)){
					NeuralNetworkLink link = NeuralNetworkNode.LinkBetweenTwoNodes(node.associatedNode, associatedNode);
					connectedNodes.Add(new Tuple<DrawableNode,Double>(node,link.weight));
				}
			}
		}

	}


}
