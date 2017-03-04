using System;

namespace Deveel.Workflows {
	/// <summary>
	/// A special activity that represents a branched execution
	/// within a parent branch (workflow or branch)
	/// </summary>
	/// <seealso cref="IBranch"/>
	/// <seealso cref="IActivity"/>
	/// <seealso cref="IActivityBranchBuilder"/>
	public interface IBranchActivity : IActivity, IBranch {
	}
}