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

## Example Code

It is possible to implement flows with two strategy of coding:

1. _Direct Moderling_: define instances of the workflow and its steps
2. _Dynamic Building Modeling_: define the workflows and steps in a virtual building process (useful for later activation of components)

``` csharp
/// Simple workflow builder
var builder = Workflow.Build(workflow => workflow               // 1.
    .Activity<Emit>()                                           // 2.
    .Activity(activity => activity                              // 3.
        .Named("do")                                            // 4.
        .If(state => state.Value is bool && (bool)c.Value)      // 5.
        .Execute((state, token) => {                            // 6.
            Console.WriteLine("Hello!"); 
            return Task.CompletedTask;
         })
     .Branch(branch => branch                                   // 7.
        .Named("branch1")                                       // 8.
        .InParallel()                                           // 9.
        .Activity<PActivity1>()                                 // 10.
        .Activity<PActivity2>()
        .Merge(BranchMergeStrategy.Instance)));                 // 11.
        
/// The context object here the an activation context provideed 
/// by the infrastructure to resolve the instance of activities
var workflow = builder.Build(context);
```

Th code above gives a general idea of a building model, and the possibilities are many more than those presented above, but in the specific:

1. The factory of a workflow builder
2. Defines a reference to a user-defined activity `Emit`, derived by `Activity` or implementing `IActivity`
3. Defines dynamically an activity using a builder
4. Gives the name ("do") to the dynamically defined activity: an activity should always be named and activity builders require the definition of the name
5. Specifies a condition to met for the activity to be executed: it takes a `State` object ar input
6. The code to be executed if the condition is met: it takes a `State` and a `CancellationToken` parameters and must return a `Task<State>` as result (potentially, in the future, alternatives will be provided to this for making more dynamic kind of results)
7. Creates a branch within the workflow
8. Gives a name to the branch: being components of a workflow, branches should be named (the builder model requires the specification of a name for branches)
9. Defines the strategy of execution of the branch: by default, the strategy applies is `Sequential`, so it is possible to change it to other strategies
10. Attaches a user-defined activity `PActivity1` to the branch that will be executed in parallel with the other activities
11. Because the branch execution strategy defined is `Parallel`, the resulting `State` will be a container of the states resulted from the parallel execution of the activities in the branch; this activity merges the states into a single state, using a user-defined strategy (`BranchMergeStrategy.Instance`) for merging the values of the states
