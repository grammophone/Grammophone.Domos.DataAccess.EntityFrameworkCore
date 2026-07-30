using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grammophone.DataAccess;
using Grammophone.Domos.Domain;

namespace Grammophone.Domos.DataAccess.EntityFrameworkCore
{
	/// <summary>
	/// A mapping class used for implicit many-to-many relations mapping in EF Core.
	/// </summary>
	/// <typeparam name="TLeft">The left-side entity type.</typeparam>
	/// <typeparam name="TRight">The right-side entity type.</typeparam>
	[Serializable]
	public class ImplicitManyToMany<TLeft, TRight> : ManyToMany<TLeft, TRight>, IPublicEntity
		where TLeft : class
		where TRight : class
	{
	}

	/// <summary>
	/// A mapping class used for implicit many-to-many relations mapping in EF Core, exposing the
	/// foreign keys as CLR properties.
	/// </summary>
	/// <typeparam name="TLeft">The left-side entity type.</typeparam>
	/// <typeparam name="TLeftKey">The primary-key type of the left-side entity.</typeparam>
	/// <typeparam name="TRight">The right-side entity type.</typeparam>
	/// <typeparam name="TRightKey">The primary-key type of the right-side entity.</typeparam>
	/// <remarks>
	/// Use this instead of <see cref="ImplicitManyToMany{TLeft, TRight}"/> when the join columns must be
	/// named — <c>j.Property(mm =&gt; mm.LeftID).HasColumnName("X_ID")</c> — which is the usual case when
	/// EF Core's default join-column naming differs from the names the schema already has.
	/// </remarks>
	[Serializable]
	public class ImplicitManyToMany<TLeft, TLeftKey, TRight, TRightKey> :
		ManyToMany<TLeft, TLeftKey, TRight, TRightKey>, IPublicEntity
		where TLeft : class
		where TRight : class
	{
	}
}
