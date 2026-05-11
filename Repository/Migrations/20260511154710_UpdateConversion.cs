using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestTasks_Quests_QuestId",
                table: "QuestTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuests_Quests_QuestId",
                table: "UserQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuests_Users_UserId",
                table: "UserQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRewards_Rewards_RewardId",
                table: "UserRewards");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRewards_Users_UserId",
                table: "UserRewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRewards",
                table: "UserRewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserQuests",
                table: "UserQuests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rewards",
                table: "Rewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestTasks",
                table: "QuestTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quests",
                table: "Quests");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Users",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "Users",
                newName: "ix_users_email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserRewards",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserRewards",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "UserRewards",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "RewardId",
                table: "UserRewards",
                newName: "reward_id");

            migrationBuilder.RenameColumn(
                name: "ReceivedAt",
                table: "UserRewards",
                newName: "received_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "UserRewards",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_UserRewards_UserId",
                table: "UserRewards",
                newName: "ix_user_rewards_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserRewards_RewardId",
                table: "UserRewards",
                newName: "ix_user_rewards_reward_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserQuests",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserQuests",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "UserQuests",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "QuestId",
                table: "UserQuests",
                newName: "quest_id");

            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "UserQuests",
                newName: "is_completed");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "UserQuests",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_UserQuests_UserId_QuestId",
                table: "UserQuests",
                newName: "ix_user_quests_user_id_quest_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserQuests_QuestId",
                table: "UserQuests",
                newName: "ix_user_quests_quest_id");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Rewards",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Rewards",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Rewards",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Rewards",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Rewards",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Rewards",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "RefreshTokens",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RefreshTokens",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RefreshTokens",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "RefreshTokens",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsRevoked",
                table: "RefreshTokens",
                newName: "is_revoked");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "RefreshTokens",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "RefreshTokens",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                newName: "ix_refresh_tokens_user_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "QuestTasks",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "QuestTasks",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "QuestTasks",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "QuestTasks",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "QuestId",
                table: "QuestTasks",
                newName: "quest_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "QuestTasks",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_QuestTasks_QuestId",
                table: "QuestTasks",
                newName: "ix_quest_tasks_quest_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Quests",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Difficulty",
                table: "Quests",
                newName: "difficulty");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Quests",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Quests",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Quests",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Quests",
                newName: "created_at");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "Users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_rewards",
                table: "UserRewards",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_quests",
                table: "UserQuests",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rewards",
                table: "Rewards",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_refresh_tokens",
                table: "RefreshTokens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_quest_tasks",
                table: "QuestTasks",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_quests",
                table: "Quests",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_quest_tasks_quests_quest_id",
                table: "QuestTasks",
                column: "quest_id",
                principalTable: "Quests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                table: "RefreshTokens",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_quests_quests_quest_id",
                table: "UserQuests",
                column: "quest_id",
                principalTable: "Quests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_quests_users_user_id",
                table: "UserQuests",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_rewards_rewards_reward_id",
                table: "UserRewards",
                column: "reward_id",
                principalTable: "Rewards",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_rewards_users_user_id",
                table: "UserRewards",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_quest_tasks_quests_quest_id",
                table: "QuestTasks");

            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "fk_user_quests_quests_quest_id",
                table: "UserQuests");

            migrationBuilder.DropForeignKey(
                name: "fk_user_quests_users_user_id",
                table: "UserQuests");

            migrationBuilder.DropForeignKey(
                name: "fk_user_rewards_rewards_reward_id",
                table: "UserRewards");

            migrationBuilder.DropForeignKey(
                name: "fk_user_rewards_users_user_id",
                table: "UserRewards");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_rewards",
                table: "UserRewards");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_quests",
                table: "UserQuests");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rewards",
                table: "Rewards");

            migrationBuilder.DropPrimaryKey(
                name: "pk_refresh_tokens",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "pk_quest_tasks",
                table: "QuestTasks");

            migrationBuilder.DropPrimaryKey(
                name: "pk_quests",
                table: "Quests");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_users_email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserRewards",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserRewards",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "UserRewards",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "reward_id",
                table: "UserRewards",
                newName: "RewardId");

            migrationBuilder.RenameColumn(
                name: "received_at",
                table: "UserRewards",
                newName: "ReceivedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "UserRewards",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_user_rewards_user_id",
                table: "UserRewards",
                newName: "IX_UserRewards_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_user_rewards_reward_id",
                table: "UserRewards",
                newName: "IX_UserRewards_RewardId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserQuests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserQuests",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "UserQuests",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "quest_id",
                table: "UserQuests",
                newName: "QuestId");

            migrationBuilder.RenameColumn(
                name: "is_completed",
                table: "UserQuests",
                newName: "IsCompleted");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "UserQuests",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_user_quests_user_id_quest_id",
                table: "UserQuests",
                newName: "IX_UserQuests_UserId_QuestId");

            migrationBuilder.RenameIndex(
                name: "ix_user_quests_quest_id",
                table: "UserQuests",
                newName: "IX_UserQuests_QuestId");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "Rewards",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Rewards",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Rewards",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Rewards",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Rewards",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Rewards",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "RefreshTokens",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RefreshTokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "RefreshTokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "RefreshTokens",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_revoked",
                table: "RefreshTokens",
                newName: "IsRevoked");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "RefreshTokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "RefreshTokens",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_user_id",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "QuestTasks",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "QuestTasks",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "QuestTasks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "QuestTasks",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "quest_id",
                table: "QuestTasks",
                newName: "QuestId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "QuestTasks",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_quest_tasks_quest_id",
                table: "QuestTasks",
                newName: "IX_QuestTasks_QuestId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Quests",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "difficulty",
                table: "Quests",
                newName: "Difficulty");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Quests",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Quests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Quests",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Quests",
                newName: "CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRewards",
                table: "UserRewards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserQuests",
                table: "UserQuests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rewards",
                table: "Rewards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestTasks",
                table: "QuestTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quests",
                table: "Quests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestTasks_Quests_QuestId",
                table: "QuestTasks",
                column: "QuestId",
                principalTable: "Quests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuests_Quests_QuestId",
                table: "UserQuests",
                column: "QuestId",
                principalTable: "Quests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuests_Users_UserId",
                table: "UserQuests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRewards_Rewards_RewardId",
                table: "UserRewards",
                column: "RewardId",
                principalTable: "Rewards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRewards_Users_UserId",
                table: "UserRewards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
