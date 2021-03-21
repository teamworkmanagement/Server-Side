﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeamApp.Infrastructure.Persistence.Entities;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(KhoaLuanContext))]
    partial class KhoaLuanContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Comment", b =>
                {
                    b.Property<string>("CommentId")
                        .HasColumnName("comment_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("CommentContent")
                        .HasColumnName("comment_content")
                        .HasColumnType("text")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<DateTime?>("CommentCreatedAt")
                        .HasColumnName("comment_created_at")
                        .HasColumnType("timestamp");

                    b.Property<bool?>("CommentIsDeleted")
                        .HasColumnName("comment_is_deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("CommentPostId")
                        .HasColumnName("comment_post_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("CommentUserId")
                        .HasColumnName("comment_user_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.HasKey("CommentId");

                    b.HasIndex("CommentPostId")
                        .HasName("comment_post_id");

                    b.HasIndex("CommentUserId")
                        .HasName("comment_user_id");

                    b.ToTable("comment");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.File", b =>
                {
                    b.Property<string>("FileId")
                        .HasColumnName("file_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("FileName")
                        .HasColumnName("file_name")
                        .HasColumnType("varchar(100)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("FileType")
                        .HasColumnName("file_type")
                        .HasColumnType("enum('word','excel','powerpoint','mp4','mp3','txt','zip','rar','others')")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("FileUrl")
                        .HasColumnName("file_url")
                        .HasColumnType("varchar(200)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.HasKey("FileId");

                    b.ToTable("file");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.GroupChat", b =>
                {
                    b.Property<string>("GroupChatId")
                        .HasColumnName("group_chat_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("GroupChatName")
                        .HasColumnName("group_chat_name")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<DateTime?>("GroupChatUpdatedAt")
                        .HasColumnName("group_chat_updated_at")
                        .HasColumnType("timestamp");

                    b.HasKey("GroupChatId");

                    b.ToTable("group_chat");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.GroupChatUser", b =>
                {
                    b.Property<string>("GroupChatUserId")
                        .HasColumnName("group_chat_user_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("GroupChatUserGroupChatId")
                        .HasColumnName("group_chat_user_group_chat_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<bool?>("GroupChatUserIsDeleted")
                        .HasColumnName("group_chat_user_is_deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("GroupChatUserUserId")
                        .HasColumnName("group_chat_user_user_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.HasKey("GroupChatUserId");

                    b.HasIndex("GroupChatUserGroupChatId")
                        .HasName("group_chat_user_group_chat_id");

                    b.HasIndex("GroupChatUserUserId")
                        .HasName("group_chat_user_user_id");

                    b.ToTable("group_chat_user");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.HandleTask", b =>
                {
                    b.Property<string>("HandleTaskId")
                        .HasColumnName("handle_task_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<DateTime?>("HandleTaskCreatedAt")
                        .HasColumnName("handle_task_created_at")
                        .HasColumnType("timestamp");

                    b.Property<bool?>("HandleTaskIsDeleted")
                        .HasColumnName("handle_task_is_deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("HandleTaskTaskId")
                        .HasColumnName("handle_task_task_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("HandleTaskUserId")
                        .HasColumnName("handle_task_user_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.HasKey("HandleTaskId");

                    b.HasIndex("HandleTaskTaskId")
                        .HasName("handle_task_task_id");

                    b.HasIndex("HandleTaskUserId")
                        .HasName("handle_task_user_id");

                    b.ToTable("handle_task");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Message", b =>
                {
                    b.Property<string>("MessageId")
                        .HasColumnName("message_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("MessageContent")
                        .HasColumnName("message_content")
                        .HasColumnType("text")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<DateTime?>("MessageCreatedAt")
                        .HasColumnName("message_created_at")
                        .HasColumnType("timestamp");

                    b.Property<string>("MessageGroupChatId")
                        .HasColumnName("message_group_chat_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<bool?>("MessageIsDeleted")
                        .HasColumnName("message_is_deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("MessageUserId")
                        .HasColumnName("message_user_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.HasKey("MessageId");

                    b.HasIndex("MessageGroupChatId")
                        .HasName("message_group_chat_id");

                    b.HasIndex("MessageUserId")
                        .HasName("message_user_id");

                    b.ToTable("message");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Notification", b =>
                {
                    b.Property<string>("NotificationId")
                        .HasColumnName("notification_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("NotificationContent")
                        .HasColumnName("notification_content")
                        .HasColumnType("text")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<DateTime?>("NotificationCreatedAt")
                        .HasColumnName("notification_created_at")
                        .HasColumnType("timestamp");

                    b.Property<bool?>("NotificationIsDeleted")
                        .HasColumnName("notification_is_deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("NotificationLink")
                        .HasColumnName("notification_link")
                        .HasColumnType("varchar(200)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<bool?>("NotificationStatus")
                        .HasColumnName("notification_status")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("NotificationUserId")
                        .HasColumnName("notification_user_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.HasKey("NotificationId");

                    b.HasIndex("NotificationUserId")
                        .HasName("notification_user_id");

                    b.ToTable("notification");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Participation", b =>
                {
                    b.Property<string>("ParticipationId")
                        .HasColumnName("participation_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<DateTime?>("ParticipationCreatedAt")
                        .HasColumnName("participation_created_at")
                        .HasColumnType("timestamp");

                    b.Property<bool?>("ParticipationIsDeleted")
                        .HasColumnName("participation_is_deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ParticipationTeamId")
                        .HasColumnName("participation_team_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("ParticipationUserId")
                        .HasColumnName("participation_user_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.HasKey("ParticipationId");

                    b.HasIndex("ParticipationTeamId")
                        .HasName("participation_team_id");

                    b.HasIndex("ParticipationUserId")
                        .HasName("participation_user_id");

                    b.ToTable("participation");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Post", b =>
                {
                    b.Property<string>("PostId")
                        .HasColumnName("post_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<int?>("PostCommentCount")
                        .HasColumnName("post_comment_count")
                        .HasColumnType("int");

                    b.Property<string>("PostContent")
                        .HasColumnName("post_content")
                        .HasColumnType("text")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<DateTime?>("PostCreatedAt")
                        .HasColumnName("post_created_at")
                        .HasColumnType("timestamp");

                    b.Property<bool?>("PostIsDeleted")
                        .HasColumnName("post_is_deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool?>("PostIsPinned")
                        .HasColumnName("post_is_pinned")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("PostTeamId")
                        .HasColumnName("post_team_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("PostUserId")
                        .HasColumnName("post_user_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.HasKey("PostId");

                    b.HasIndex("PostTeamId")
                        .HasName("post_team_id");

                    b.HasIndex("PostUserId")
                        .HasName("post_user_id");

                    b.ToTable("post");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Tag", b =>
                {
                    b.Property<string>("TagId")
                        .HasColumnName("tag_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("TagContent")
                        .HasColumnName("tag_content")
                        .HasColumnType("text")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("TagLink")
                        .HasColumnName("tag_link")
                        .HasColumnType("varchar(200)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.HasKey("TagId");

                    b.ToTable("tag");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Task", b =>
                {
                    b.Property<string>("TaskId")
                        .HasColumnName("task_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<int?>("TaskCompletedPercent")
                        .HasColumnName("task_completed_percent")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TaskCreatedAt")
                        .HasColumnName("task_created_at")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("TaskDeadline")
                        .HasColumnName("task_deadline")
                        .HasColumnType("timestamp");

                    b.Property<string>("TaskDescription")
                        .HasColumnName("task_description")
                        .HasColumnType("varchar(500)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<bool?>("TaskIsDeleted")
                        .HasColumnName("task_is_deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("TaskName")
                        .HasColumnName("task_name")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<int?>("TaskPoint")
                        .HasColumnName("task_point")
                        .HasColumnType("int");

                    b.Property<string>("TaskStatus")
                        .HasColumnName("task_status")
                        .HasColumnType("enum('todo','doing','done')")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("TaskTeamId")
                        .HasColumnName("task_team_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.HasKey("TaskId");

                    b.HasIndex("TaskTeamId")
                        .HasName("task_team_id");

                    b.ToTable("task");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.TaskVersion", b =>
                {
                    b.Property<string>("TaskVersionId")
                        .HasColumnName("task_version_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<int?>("TaskVersionTaskCompletedPercent")
                        .HasColumnName("task_version_task_completed_percent")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TaskVersionTaskDeadline")
                        .HasColumnName("task_version_task_deadline")
                        .HasColumnType("timestamp");

                    b.Property<string>("TaskVersionTaskDescription")
                        .HasColumnName("task_version_task_description")
                        .HasColumnType("varchar(500)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("TaskVersionTaskId")
                        .HasColumnName("task_version_task_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<bool?>("TaskVersionTaskIsDeleted")
                        .HasColumnName("task_version_task_is_deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("TaskVersionTaskName")
                        .HasColumnName("task_version_task_name")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<int?>("TaskVersionTaskPoint")
                        .HasColumnName("task_version_task_point")
                        .HasColumnType("int");

                    b.Property<string>("TaskVersionTaskStatus")
                        .HasColumnName("task_version_task_status")
                        .HasColumnType("enum('todo','doing','done')")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<DateTime?>("TaskVersionUpdatedAt")
                        .HasColumnName("task_version_updated_at")
                        .HasColumnType("timestamp");

                    b.HasKey("TaskVersionId");

                    b.HasIndex("TaskVersionTaskId")
                        .HasName("task_version_task_id");

                    b.ToTable("task_version");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Team", b =>
                {
                    b.Property<string>("TeamId")
                        .HasColumnName("team_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("TeamCode")
                        .HasColumnName("team_code")
                        .HasColumnType("varchar(20)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<DateTime?>("TeamCreatedAt")
                        .HasColumnName("team_created_at")
                        .HasColumnType("timestamp");

                    b.Property<string>("TeamDescription")
                        .HasColumnName("team_description")
                        .HasColumnType("varchar(500)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<bool?>("TeamIsDeleted")
                        .HasColumnName("team_is_deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("TeamLeaderId")
                        .HasColumnName("team_leader_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("TeamName")
                        .HasColumnName("team_name")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.HasKey("TeamId");

                    b.HasIndex("TeamLeaderId")
                        .HasName("team_leader_id");

                    b.ToTable("team");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("UsePhoneNumber")
                        .HasColumnName("use_phone_number")
                        .HasColumnType("varchar(20)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<DateTime?>("UserCreatedAt")
                        .HasColumnName("user_created_at")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("UserDateOfBirth")
                        .HasColumnName("user_date_of_birth")
                        .HasColumnType("timestamp");

                    b.Property<string>("UserEmail")
                        .HasColumnName("user_email")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("UserFullname")
                        .HasColumnName("user_fullname")
                        .HasColumnType("varchar(100)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<string>("UserImageUrl")
                        .HasColumnName("user_image_url")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.Property<bool?>("UserIsThemeLight")
                        .HasColumnName("user_is_theme_light")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserPassword")
                        .HasColumnName("user_password")
                        .HasColumnType("varchar(50)")
                        .HasAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .HasAnnotation("MySql:CharSet", "utf8mb4");

                    b.HasKey("UserId");

                    b.ToTable("user");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Comment", b =>
                {
                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.Post", "CommentPost")
                        .WithMany("Comment")
                        .HasForeignKey("CommentPostId")
                        .HasConstraintName("comment_ibfk_1");

                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.User", "CommentUser")
                        .WithMany("Comment")
                        .HasForeignKey("CommentUserId")
                        .HasConstraintName("comment_ibfk_2");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.GroupChatUser", b =>
                {
                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.GroupChat", "GroupChatUserGroupChat")
                        .WithMany("GroupChatUser")
                        .HasForeignKey("GroupChatUserGroupChatId")
                        .HasConstraintName("group_chat_user_ibfk_2");

                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.User", "GroupChatUserUser")
                        .WithMany("GroupChatUser")
                        .HasForeignKey("GroupChatUserUserId")
                        .HasConstraintName("group_chat_user_ibfk_1");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.HandleTask", b =>
                {
                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.Task", "HandleTaskTask")
                        .WithMany("HandleTask")
                        .HasForeignKey("HandleTaskTaskId")
                        .HasConstraintName("handle_task_ibfk_2");

                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.User", "HandleTaskUser")
                        .WithMany("HandleTask")
                        .HasForeignKey("HandleTaskUserId")
                        .HasConstraintName("handle_task_ibfk_1");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Message", b =>
                {
                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.GroupChat", "MessageGroupChat")
                        .WithMany("Message")
                        .HasForeignKey("MessageGroupChatId")
                        .HasConstraintName("message_ibfk_2");

                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.User", "MessageUser")
                        .WithMany("Message")
                        .HasForeignKey("MessageUserId")
                        .HasConstraintName("message_ibfk_1");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Notification", b =>
                {
                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.User", "NotificationUser")
                        .WithMany("Notification")
                        .HasForeignKey("NotificationUserId")
                        .HasConstraintName("notification_ibfk_1");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Participation", b =>
                {
                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.Team", "ParticipationTeam")
                        .WithMany("Participation")
                        .HasForeignKey("ParticipationTeamId")
                        .HasConstraintName("participation_ibfk_2");

                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.User", "ParticipationUser")
                        .WithMany("Participation")
                        .HasForeignKey("ParticipationUserId")
                        .HasConstraintName("participation_ibfk_1");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Post", b =>
                {
                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.Team", "PostTeam")
                        .WithMany("Post")
                        .HasForeignKey("PostTeamId")
                        .HasConstraintName("post_ibfk_2");

                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.User", "PostUser")
                        .WithMany("Post")
                        .HasForeignKey("PostUserId")
                        .HasConstraintName("post_ibfk_1");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Task", b =>
                {
                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.Team", "TaskTeam")
                        .WithMany("Task")
                        .HasForeignKey("TaskTeamId")
                        .HasConstraintName("task_ibfk_1");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.TaskVersion", b =>
                {
                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.Task", "TaskVersionTask")
                        .WithMany("TaskVersion")
                        .HasForeignKey("TaskVersionTaskId")
                        .HasConstraintName("task_version_ibfk_1");
                });

            modelBuilder.Entity("TeamApp.Infrastructure.Persistence.Entities.Team", b =>
                {
                    b.HasOne("TeamApp.Infrastructure.Persistence.Entities.User", "TeamLeader")
                        .WithMany("Team")
                        .HasForeignKey("TeamLeaderId")
                        .HasConstraintName("team_ibfk_1");
                });
#pragma warning restore 612, 618
        }
    }
}
