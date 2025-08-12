using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumo.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddFullTextSearchColumn : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Add search_vector column
        migrationBuilder.Sql(@"
            ALTER TABLE stories 
            ADD COLUMN search_vector tsvector;
        ");

        // Create function to update search vector
        migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION update_story_search_vector()
            RETURNS TRIGGER AS $$
            BEGIN
                NEW.search_vector := 
                    setweight(to_tsvector('english', COALESCE(NEW.title, '')), 'A') ||
                    setweight(to_tsvector('english', COALESCE(NEW.content, '')), 'B');
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;
        ");

        // Create trigger
        migrationBuilder.Sql(@"
            CREATE TRIGGER story_search_vector_update
                BEFORE INSERT OR UPDATE ON stories
                FOR EACH ROW EXECUTE FUNCTION update_story_search_vector();
        ");

        // Update existing records
        migrationBuilder.Sql(@"
            UPDATE stories SET search_vector = 
                setweight(to_tsvector('english', COALESCE(title, '')), 'A') ||
                setweight(to_tsvector('english', COALESCE(content, '')), 'B');
        ");

        // Create GIN index for fast full-text search
        migrationBuilder.Sql(@"
            CREATE INDEX IX_Stories_SearchVector ON stories USING gin(search_vector);
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TRIGGER IF EXISTS story_search_vector_update ON stories;");
        migrationBuilder.Sql("DROP FUNCTION IF EXISTS update_story_search_vector();");
        migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Stories_SearchVector;");
        migrationBuilder.Sql("ALTER TABLE stories DROP COLUMN IF EXISTS search_vector;");
    }
}
