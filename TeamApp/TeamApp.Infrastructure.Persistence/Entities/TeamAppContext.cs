﻿using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class TeamAppContext : IdentityDbContext<User>
    {
        public TeamAppContext()
        {
        }

        public TeamAppContext(DbContextOptions<TeamAppContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<File> File { get; set; }
        public virtual DbSet<GroupChat> GroupChat { get; set; }
        public virtual DbSet<GroupChatUser> GroupChatUser { get; set; }
        public virtual DbSet<HandleTask> HandleTask { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<Participation> Participation { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<Task> Task { get; set; }
        public virtual DbSet<TaskVersion> TaskVersion { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserConnection> UserConnection { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_token");
            });

            modelBuilder.Entity<UserConnection>(entity =>
            {
                entity.ToTable("user_connection");
                entity.HasKey(e => new { e.UserId, e.ConnectionId });
                entity.Property(e => e.ConnectionId)
                    .HasColumnName("user_connection_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comment");

                entity.HasIndex(e => e.CommentPostId)
                    .HasName("comment_post_id");

                entity.HasIndex(e => e.CommentUserId)
                    .HasName("comment_user_id");

                entity.Property(e => e.CommentId)
                    .HasColumnName("comment_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.CommentContent)
                    .HasColumnName("comment_content")
                    .HasColumnType("text")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.CommentCreatedAt)
                    .HasColumnName("comment_created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.CommentIsDeleted).HasColumnName("comment_is_deleted");

                entity.Property(e => e.CommentPostId)
                    .HasColumnName("comment_post_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.CommentUserId)
                    .HasColumnName("comment_user_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.CommentType)
                    .HasColumnName("comment_type")
                    .HasColumnType("enum('text','file')")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.HasOne(d => d.CommentPost)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.CommentPostId)
                    .HasConstraintName("comment_ibfk_1");

                entity.HasOne(d => d.CommentUser)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.CommentUserId)
                    .HasConstraintName("comment_ibfk_2");
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.ToTable("file");

                entity.Property(e => e.FileId)
                    .HasColumnName("file_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.FileName)
                    .HasColumnName("file_name")
                    .HasColumnType("varchar(100)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.FileType)
                    .HasColumnName("file_type")
                    .HasColumnType("enum('word','excel','powerpoint','mp4','mp3','txt','zip','rar','others')")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.FileUrl)
                    .HasColumnName("file_url")
                    .HasColumnType("varchar(200)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");
            });

            modelBuilder.Entity<GroupChat>(entity =>
            {
                entity.ToTable("group_chat");

                entity.Property(e => e.GroupChatId)
                    .HasColumnName("group_chat_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.GroupChatName)
                    .HasColumnName("group_chat_name")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.GroupChatUpdatedAt)
                    .HasColumnName("group_chat_updated_at")
                    .HasColumnType("timestamp");
            });

            modelBuilder.Entity<GroupChatUser>(entity =>
            {
                entity.ToTable("group_chat_user");

                entity.HasIndex(e => e.GroupChatUserGroupChatId)
                    .HasName("group_chat_user_group_chat_id");

                entity.HasIndex(e => e.GroupChatUserUserId)
                    .HasName("group_chat_user_user_id");

                entity.Property(e => e.GroupChatUserId)
                    .HasColumnName("group_chat_user_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.GroupChatUserGroupChatId)
                    .HasColumnName("group_chat_user_group_chat_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.GroupChatUserIsDeleted).HasColumnName("group_chat_user_is_deleted");

                entity.Property(e => e.GroupChatUserUserId)
                    .HasColumnName("group_chat_user_user_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.GroupChatUserSeen).HasColumnName("group_chat_user_seen");

                entity.HasOne(d => d.GroupChatUserGroupChat)
                    .WithMany(p => p.GroupChatUser)
                    .HasForeignKey(d => d.GroupChatUserGroupChatId)
                    .HasConstraintName("group_chat_user_ibfk_2");

                entity.HasOne(d => d.GroupChatUserUser)
                    .WithMany(p => p.GroupChatUser)
                    .HasForeignKey(d => d.GroupChatUserUserId)
                    .HasConstraintName("group_chat_user_ibfk_1");
            });

            modelBuilder.Entity<HandleTask>(entity =>
            {
                entity.ToTable("handle_task");

                entity.HasIndex(e => e.HandleTaskTaskId)
                    .HasName("handle_task_task_id");

                entity.HasIndex(e => e.HandleTaskUserId)
                    .HasName("handle_task_user_id");

                entity.Property(e => e.HandleTaskId)
                    .HasColumnName("handle_task_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.HandleTaskCreatedAt)
                    .HasColumnName("handle_task_created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.HandleTaskIsDeleted).HasColumnName("handle_task_is_deleted");

                entity.Property(e => e.HandleTaskTaskId)
                    .HasColumnName("handle_task_task_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.HandleTaskUserId)
                    .HasColumnName("handle_task_user_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.HasOne(d => d.HandleTaskTask)
                    .WithMany(p => p.HandleTask)
                    .HasForeignKey(d => d.HandleTaskTaskId)
                    .HasConstraintName("handle_task_ibfk_2");

                entity.HasOne(d => d.HandleTaskUser)
                    .WithMany(p => p.HandleTask)
                    .HasForeignKey(d => d.HandleTaskUserId)
                    .HasConstraintName("handle_task_ibfk_1");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("message");

                entity.HasIndex(e => e.MessageGroupChatId)
                    .HasName("message_group_chat_id");

                entity.HasIndex(e => e.MessageUserId)
                    .HasName("message_user_id");

                entity.Property(e => e.MessageId)
                    .HasColumnName("message_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.MessageContent)
                    .HasColumnName("message_content")
                    .HasColumnType("text")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.MessageCreatedAt)
                    .HasColumnName("message_created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.MessageGroupChatId)
                    .HasColumnName("message_group_chat_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.MessageType)
                    .HasColumnName("message_type")
                    .HasColumnType("enum('text','file')")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.MessageIsDeleted).HasColumnName("message_is_deleted");

                entity.Property(e => e.IsMessage).HasColumnName("is_message");

                entity.Property(e => e.MessageUserId)
                    .HasColumnName("message_user_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.HasOne(d => d.MessageGroupChat)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.MessageGroupChatId)
                    .HasConstraintName("message_ibfk_2");

                entity.HasOne(d => d.MessageUser)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.MessageUserId)
                    .HasConstraintName("message_ibfk_1");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notification");

                entity.HasIndex(e => e.NotificationUserId)
                    .HasName("notification_user_id");

                entity.Property(e => e.NotificationId)
                    .HasColumnName("notification_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.NotificationContent)
                    .HasColumnName("notification_content")
                    .HasColumnType("text")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.NotificationCreatedAt)
                    .HasColumnName("notification_created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.NotificationIsDeleted).HasColumnName("notification_is_deleted");

                entity.Property(e => e.NotificationLink)
                    .HasColumnName("notification_link")
                    .HasColumnType("varchar(200)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.NotificationStatus).HasColumnName("notification_status");

                entity.Property(e => e.NotificationUserId)
                    .HasColumnName("notification_user_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.HasOne(d => d.NotificationUser)
                    .WithMany(p => p.Notification)
                    .HasForeignKey(d => d.NotificationUserId)
                    .HasConstraintName("notification_ibfk_1");
            });

            modelBuilder.Entity<Participation>(entity =>
            {
                entity.ToTable("participation");

                entity.HasIndex(e => e.ParticipationTeamId)
                    .HasName("participation_team_id");

                entity.HasIndex(e => e.ParticipationUserId)
                    .HasName("participation_user_id");

                entity.Property(e => e.ParticipationId)
                    .HasColumnName("participation_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.ParticipationCreatedAt)
                    .HasColumnName("participation_created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.ParticipationIsDeleted).HasColumnName("participation_is_deleted");

                entity.Property(e => e.ParticipationTeamId)
                    .HasColumnName("participation_team_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.ParticipationUserId)
                    .HasColumnName("participation_user_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.HasOne(d => d.ParticipationTeam)
                    .WithMany(p => p.Participation)
                    .HasForeignKey(d => d.ParticipationTeamId)
                    .HasConstraintName("participation_ibfk_2");

                entity.HasOne(d => d.ParticipationUser)
                    .WithMany(p => p.Participation)
                    .HasForeignKey(d => d.ParticipationUserId)
                    .HasConstraintName("participation_ibfk_1");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("post");

                entity.HasIndex(e => e.PostTeamId)
                    .HasName("post_team_id");

                entity.HasIndex(e => e.PostUserId)
                    .HasName("post_user_id");

                entity.Property(e => e.PostId)
                    .HasColumnName("post_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.PostCommentCount).HasColumnName("post_comment_count");

                entity.Property(e => e.PostContent)
                    .HasColumnName("post_content")
                    .HasColumnType("text")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.PostCreatedAt)
                    .HasColumnName("post_created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.PostIsDeleted).HasColumnName("post_is_deleted");

                entity.Property(e => e.PostIsPinned).HasColumnName("post_is_pinned");

                entity.Property(e => e.PostTeamId)
                    .HasColumnName("post_team_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.PostUserId)
                    .HasColumnName("post_user_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.HasOne(d => d.PostTeam)
                    .WithMany(p => p.Post)
                    .HasForeignKey(d => d.PostTeamId)
                    .HasConstraintName("post_ibfk_2");

                entity.HasOne(d => d.PostUser)
                    .WithMany(p => p.Post)
                    .HasForeignKey(d => d.PostUserId)
                    .HasConstraintName("post_ibfk_1");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tag");

                entity.Property(e => e.TagId)
                    .HasColumnName("tag_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TagContent)
                    .HasColumnName("tag_content")
                    .HasColumnType("text")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TagLink)
                    .HasColumnName("tag_link")
                    .HasColumnType("varchar(200)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.ToTable("task");

                entity.HasIndex(e => e.TaskTeamId)
                    .HasName("task_team_id");

                entity.Property(e => e.TaskId)
                    .HasColumnName("task_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TaskCompletedPercent).HasColumnName("task_completed_percent");

                entity.Property(e => e.TaskCreatedAt)
                    .HasColumnName("task_created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.TaskDeadline)
                    .HasColumnName("task_deadline")
                    .HasColumnType("timestamp");

                entity.Property(e => e.TaskDescription)
                    .HasColumnName("task_description")
                    .HasColumnType("varchar(500)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TaskIsDeleted).HasColumnName("task_is_deleted");

                entity.Property(e => e.TaskName)
                    .HasColumnName("task_name")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TaskPoint).HasColumnName("task_point");

                entity.Property(e => e.TaskStatus)
                    .HasColumnName("task_status")
                    .HasColumnType("enum('todo','doing','done')")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TaskTeamId)
                    .HasColumnName("task_team_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.HasOne(d => d.TaskTeam)
                    .WithMany(p => p.Task)
                    .HasForeignKey(d => d.TaskTeamId)
                    .HasConstraintName("task_ibfk_1");
            });

            modelBuilder.Entity<TaskVersion>(entity =>
            {
                entity.ToTable("task_version");

                entity.HasIndex(e => e.TaskVersionTaskId)
                    .HasName("task_version_task_id");

                entity.Property(e => e.TaskVersionId)
                    .HasColumnName("task_version_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TaskVersionTaskCompletedPercent).HasColumnName("task_version_task_completed_percent");

                entity.Property(e => e.TaskVersionTaskDeadline)
                    .HasColumnName("task_version_task_deadline")
                    .HasColumnType("timestamp");

                entity.Property(e => e.TaskVersionTaskDescription)
                    .HasColumnName("task_version_task_description")
                    .HasColumnType("varchar(500)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TaskVersionTaskId)
                    .HasColumnName("task_version_task_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TaskVersionTaskIsDeleted).HasColumnName("task_version_task_is_deleted");

                entity.Property(e => e.TaskVersionTaskName)
                    .HasColumnName("task_version_task_name")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TaskVersionTaskPoint).HasColumnName("task_version_task_point");

                entity.Property(e => e.TaskVersionTaskStatus)
                    .HasColumnName("task_version_task_status")
                    .HasColumnType("enum('todo','doing','done')")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TaskVersionUpdatedAt)
                    .HasColumnName("task_version_updated_at")
                    .HasColumnType("timestamp");

                entity.HasOne(d => d.TaskVersionTask)
                    .WithMany(p => p.TaskVersion)
                    .HasForeignKey(d => d.TaskVersionTaskId)
                    .HasConstraintName("task_version_ibfk_1");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("team");

                entity.HasIndex(e => e.TeamLeaderId)
                    .HasName("team_leader_id");

                entity.Property(e => e.TeamId)
                    .HasColumnName("team_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TeamCode)
                    .HasColumnName("team_code")
                    .HasColumnType("varchar(20)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TeamCreatedAt)
                    .HasColumnName("team_created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.TeamDescription)
                    .HasColumnName("team_description")
                    .HasColumnType("varchar(500)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TeamIsDeleted).HasColumnName("team_is_deleted");

                entity.Property(e => e.TeamLeaderId)
                    .HasColumnName("team_leader_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.TeamName)
                    .HasColumnName("team_name")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.HasOne(d => d.TeamLeader)
                    .WithMany(p => p.Team)
                    .HasForeignKey(d => d.TeamLeaderId)
                    .HasConstraintName("team_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id)
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("use_phone_number")
                    .HasColumnType("varchar(20)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("user_created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Dob)
                    .HasColumnName("user_date_of_birth")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Email)
                    .HasColumnName("user_email")
                    .HasColumnType("varchar(50)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.FullName)
                    .HasColumnName("user_fullname")
                    .HasColumnType("varchar(100)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("user_image_url")
                    .HasColumnType("varchar(500)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.IsThemeLight).HasColumnName("user_is_theme_light");

                entity.Property(e => e.PasswordHash)
                    .HasColumnName("user_password")
                    .HasColumnType("varchar(500)")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasCharSet("utf8mb4");
            });


            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("role");
                entity.Property(m => m.Id).HasMaxLength(85);
                entity.Property(m => m.NormalizedName).HasMaxLength(85);
            });
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("user_roles");
                entity.Property(m => m.RoleId).HasMaxLength(50);
                entity.Property(m => m.UserId).HasMaxLength(50);
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("user_claims");
                entity.Property(m => m.Id).HasMaxLength(50);
                entity.Property(m => m.UserId).HasMaxLength(50);
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("user_logins");
                entity.HasKey(e => e.UserId);
                entity.Property(m => m.LoginProvider).HasMaxLength(50);
                entity.Property(m => m.ProviderKey).HasMaxLength(50);
                entity.Property(m => m.UserId).HasMaxLength(50);
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("role_claims");
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(m => m.RoleId).HasMaxLength(50);
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("user_tokens");
                entity.Property(m => m.UserId).HasMaxLength(50);
                entity.Property(m => m.LoginProvider).HasMaxLength(50);
                entity.Property(m => m.Name).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
