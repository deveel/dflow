# DFlow

This is a lightweight library, with no aim to be an enterprise-level solution, to easily design in-code workflows to operate from an initial state to a final state.

## Workflows

The workflow model offered by DFlow is simple and is read-only: once defined the workflow model is immutable.

```
Worflow
+- Activity1
+- Activity2
+- Branch1
+--- Branch-Activity1
+--- Branch-Activity2
```

A workflow is designed as a container of activities and follows a sequential order of execution (that means, the result of an activity is the input of the next activity)
Branches to the workflow can be defined with other strategies of execution (the provided ones are _parallel_ and _sequential_)

## Activities

Activities are structured in a decision-tree model

* Is the activity executable given the current state?
* Execute the activity and return a new state next to the current

There are four kind of pre-defined activities in DFlow

1. **Execution Activity**: is the foundation activity for decision making and executing of a step of the workflow. In fact, all other activities derive from this
2. **Branch Activity**: creates a branch of activities to execute within a workflow or othr branches
3. **Merge Activity**: an activitiy that merges the resulting state of a parallel executed branch, using a strategy defined by the user, to come out with a single result
4. **Repeat Activity**: given a decisor (a logic to evaluate the current state), this activity repeat the current branch until the condition of the decisor is met (much like a _do-while_ logic)

It is possible to implement new activities using the library: in fact all the activities derive from the _Execution Activity_
