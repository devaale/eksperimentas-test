using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.EntityFrameworkCore;

using Experiment.Core;
using Experiment.Data.Models;
using MD = Experiment.Data.Metadata;

using Website.Data;
using System.ComponentModel;
using System;

namespace Website.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class
	// please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : IdentityUser, MD.IUser
	{
		[Required]
		[StringLength(128)]
		public string Name { get; set; }

		[Required]
		[StringLength(3)]
		//[Column(TypeName = "VARCHAR(3)")]	// This messing all things up
		public string Language { get; set; }

		[DefaultValue(0)]
		public int Tokens { get; set; }

		[DefaultValue(false)]
		public bool IsAdmin { get; set; }

		/// <summary>
		/// Blockchain address of an user
		/// </summary>
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public string Address { get; set; }

		[DefaultValue(null)]
		public DateTime? RemovalRequested { get; set; }

		//[DefaultValue(null)]public string ShData { get; set; }
		//[DefaultValue(null)]public string ShAddress { get; set; }

		/// <summary>
		/// Web auth (build-in)
		/// </summary>
		/// <param name="manager"></param>
		/// <returns></returns>
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}

		/// <summary>
		/// External client auth (added)
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="authenticationType"></param>
		/// <returns></returns>
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
			// Add custom user claims here
			return userIdentity;
		}
	}

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
			Configuration.LazyLoadingEnabled = false;
		}

		/// <summary>
		/// Added D.S. 2023.07.27 to solve EF decimal(18, 4) precision problem
		/// 
		/// @see:
		/// https://stackoverflow.com/a/52688314
		/// https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties?tabs=fluent-api%2Cwithout-nrt#tabpanel_5_fluent-api
		/// </summary>
		/// <param name="modelBuilder"></param>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Datapoint>()
				.Property(x => x.Multiplier)
				.HasPrecision(18, 4);
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		public System.Data.Entity.DbSet<Website.Models.EObject> Objects { get; set; }

		public System.Data.Entity.DbSet<Website.Models.ObjectPermission> ObjectPermissions { get; set; }

        public System.Data.Entity.DbSet<Website.Models.Group> Groups { get; set; }

        public System.Data.Entity.DbSet<Website.Models.Algorithm> Algorithms { get; set; }

        public System.Data.Entity.DbSet<Website.Models.Device> Devices { get; set; }

		public System.Data.Entity.DbSet<Website.Models.Datapoint> Datapoints { get; set; }

		public System.Data.Entity.DbSet<Website.Models.GroupDatapoint> GroupDatapoints { get; set; }

		public System.Data.Entity.DbSet<Website.Models.DatapointValue> DatapointValues { get; set; }

		public System.Data.Entity.DbSet<Website.Models.Language> Languages { get; set; }

		public System.Data.Entity.DbSet<Website.Models.Word> Words { get; set; }

		public System.Data.Entity.DbSet<Website.Models.Friend> Friends { get; set; }

		public System.Data.Entity.DbSet<Website.Models.Blocked> Blocked { get; set; }

		public System.Data.Entity.DbSet<Website.Models.Post> Posts { get; set; }

		public System.Data.Entity.DbSet<Website.Models.PostImage> PostImages { get; set; }

		public System.Data.Entity.DbSet<Website.Models.PostReaction> PostReactions { get; set; }

		public System.Data.Entity.DbSet<Website.Models.Message> Messages { get; set; }

		public System.Data.Entity.DbSet<Website.Models.TokenTransaction> TokenTransactions { get; set; }

		public System.Data.Entity.DbSet<Website.Models.License> Licenses { get; set; }

		public System.Data.Entity.DbSet<Website.Models.Order> Orders { get; set; }

		public System.Data.Entity.DbSet<Website.Models.OrderDetail> OrderDetails { get; set; }

		public System.Data.Entity.DbSet<Website.Models.DatapointFormula> DatapointFormulas { get; set; }

		public System.Data.Entity.DbSet<Website.Models.DatapointFormulaChain> DatapointFormulaChains { get; set; }

		public System.Data.Entity.DbSet<Website.Models.ReportRequest> ReportRequests { get; set; }

		public System.Data.Entity.DbSet<Website.Models.Wallet> Wallets { get; set; }

		public System.Data.Entity.DbSet<Website.Models.BlockchainLog> BlockchainLogs { get; set; }

		public System.Data.Entity.DbSet<Website.Models.DashboardSetting> DashboardSettings { get; set; }

		public System.Data.Entity.DbSet<Website.Models.DashboardDatapoint> DashboardDatapoints { get; set; }

		public System.Data.Entity.DbSet<Website.Models.DeviceTopic> DeviceTopics { get; set; }

		public System.Data.Entity.DbSet<Website.Models.DatapointSetting> DatapointSettings { get; set; }
	}
}
