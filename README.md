# Pendulum-balancing-ai
A simple implementation of the NEAT AI algorithm in Godot with C#.

This project demonstrates reinforcement learning via a self balancing pendulum.
The idea is that there is a pendulum which is locked to move left and right
on a fixed axis. In order to balance the pendulum it can move left and right
and wave the pendulum while doing so, which can be used to balance it upright.

## Training Process

Upon starting a batch of pendulums are created and they have random behaviour.
They are evaluated by their ability to remain upright. Each agent is awarded
points during the time they are upright and their score is removed while they don't
remain upright.

After the training time the agents are filtered based on score and the ons who
fall in the lower percentile undergoe a mutation. This process repeats each generation
and the overall preformance increases.

## Percentage of agents to mutate
This slider represents the percentile of the agents that will undergoe a mutation. If an agent falls into this percentile based on their preformance they will be mutated, if not they will remain the same and take part in the next generation.

## Batch size
This represents the number of agents to spawn per generation.

## Neural network visualizer
The dots represent the indivil neurons of the neural network.
The lines represent links between nodes, red ones are positively weighted and blue ones are negatively weighted. The line thicknes represents the strength of the weight. A thin line would be close to 0 while a thick line will be a larger number.
