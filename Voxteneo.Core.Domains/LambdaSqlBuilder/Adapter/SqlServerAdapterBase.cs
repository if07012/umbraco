/* License: http://www.apache.org/licenses/LICENSE-2.0 */

namespace Voxteneo.Core.Domains.LambdaSqlBuilder.Adapter
{
    /// <summary>
    /// Provides functionality common to all supported SQL Server versions
    /// </summary>
    class SqlServerAdapterBase : SqlAdapterBase
    {
        public string QueryStringPage(string source, string selection, string conditions, string order,
            int pageSize)
        {
            return string.Format("SELECT TOP({4}) {0} FROM {1} {2} {3}",
                    selection, source, conditions, order, pageSize);
        }


        public string Table(string tableName)
        {
            return string.Format("[{0}]", tableName);
        }

        public string Field(string tableName, string fieldName)
        {
            return string.Format("[{0}].[{1}]", tableName, fieldName);
        }

        public string Parameter(string parameterId)
        {
            return "@" + parameterId;
        }

        public string CreateCondition(string tableName, string fieldName, string paramId, string op)
        {
            return string.Format("{0} {1} {2}",
                    Field(tableName, fieldName),
                    op,
                    Parameter(paramId));
        }
        public string CreateConditionComparison(string leftTableName, string leftFieldName, string rightTableName,
          string rightFieldName, string op)
        {
            return string.Format("{0} {1} {2}",
            Field(leftTableName, leftFieldName),
            op,
            Field(rightTableName, rightFieldName));
        }
        public string CreateConditionFieldNotNull(string tableName, string fieldName)
        {
            return string.Format(" {0} IS NOT NULL", Field(tableName, fieldName) + " ");
        }
    }
}
