using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Extractor.iOS.Entity;
using SQLite;

namespace Extractor.iOS.Query
{
    internal sealed class GetGroupConversationsQuery : IQuery<List<DbGroupChatMessage>>
    {
        private readonly SQLiteAsyncConnection connection;

        public GetGroupConversationsQuery(SQLiteAsyncConnection connection)
        {
            this.connection = connection;
        }

        public Task<List<DbGroupChatMessage>> Execute(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.connection.QueryAsync<DbGroupChatMessage>(@"
                SELECT m.ROWID AS message_id, h.id AS handle, c.service_name, c.group_id
                FROM message m
                JOIN chat_message_join cmj ON cmj.message_id = m.rowid
                JOIN chat_handle_join chj ON chj.chat_id = cmj.chat_id
                JOIN handle h ON h.rowid = chj.handle_id
                JOIN chat c ON c.rowid = cmj.chat_id
                WHERE cmj.chat_id IN (
	                SELECT chat_id
	                FROM chat_handle_join
	                GROUP BY chat_id
	                HAVING COUNT(handle_id) > 1)
                ORDER BY cmj.chat_id");
        }
    }
}