/* License: http://www.apache.org/licenses/LICENSE-2.0 */

namespace Voxteneo.Core.Domains.LambdaSqlBuilder.Adapter
{
    /// <summary>
    /// SQL adapter provides db specific functionality related to db specific SQL syntax
    /// </summary>
    public interface ISqlAdapter
    {
        string QueryString(string selection, string source, string conditions, 
            string order, string grouping, string having);

        string QueryStringPage(string selection, string source, string conditions, string order,
            int pageSize, int pageNumber);

        string QueryStringPage(string selection, string source, string conditions, string order,
            int pageSize);

        string Table(string tableName);
        string Field(string tableName, string fieldName);
        string Parameter(string parameterId);
        string CreateCondition(string tableName, string fieldName, string paramId, string op);

        string CreateConditionComparison(string leftTableName, string leftFieldName, string rightTableName,
            string rightFieldName, string op);

        string CreateConditionFieldNotNull(string tableName, string fieldName);
    }
}
