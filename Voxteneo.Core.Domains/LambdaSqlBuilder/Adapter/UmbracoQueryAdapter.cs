using System.Collections.Generic;
using System.Linq.Expressions;
using Voxteneo.Core.Domains.LambdaSqlBuilder.Resolver;

namespace Voxteneo.Core.Domains.LambdaSqlBuilder.Adapter
{
    public class UmbracoQueryAdapter : ISqlAdapter
    {
        public UmbracoQueryAdapter()
        {
            LambdaResolver._operationDictionary = new Dictionary<ExpressionType, string>()
                                                                              {
                                                                                  { ExpressionType.Equal, "="},
                                                                                  { ExpressionType.NotEqual, "!="},
                                                                                  { ExpressionType.GreaterThan, ">"},
                                                                                  { ExpressionType.LessThan, "<"},
                                                                                  { ExpressionType.GreaterThanOrEqual, ">="},
                                                                                  { ExpressionType.LessThanOrEqual, "<="}
                                                                              };
        }
        public string QueryString(string selection, string source, string conditions, string order, string grouping, string having)
        {
            selection = selection.Replace("cmsContentXml.*", "cmsContentXml.xml");
            return string.Format("SELECT {0} FROM {1} {2} {3} {4} {5}",
                                 selection, source, conditions, order, grouping, having);
        }

        public string QueryStringPage(string selection, string source, string conditions, string order, int pageSize, int pageNumber)
        {
            selection = selection.Replace("cmsContentXml.*", "cmsContentXml.xml");
            return string.Format("SELECT TOP({4}) {0} FROM {1} {2} {3}",
                    selection, source, conditions, order, pageSize);
        }

        public string QueryStringPage(string selection, string source, string conditions, string order, int pageSize)
        {
            throw new System.NotImplementedException();
        }

        public string Table(string tableName)
        {
            return "cmsContentXml";
        }

        public string Field(string tableName, string fieldName)
        {
            return "dbo.GetContent(cmsContentXml.xml,'" + fieldName + "')";
        }

        public string Parameter(string parameterId)
        {
            return "@" + parameterId;
        }

        public string CreateCondition(string tableName, string fieldName, string paramId, string op)
        {
            return string.Format("(cmsContentXml.xml like '<" + tableName + "%' and {0} {1} {2})",
                   Field(tableName, fieldName),
                   op,
                   Parameter(paramId));
        }

        public string CreateConditionComparison(string leftTableName, string leftFieldName, string rightTableName,
            string rightFieldName, string op)
        {
            return
            string.Format("{0} {1} {2}",
            Field(leftTableName, leftFieldName),
            op,
            Field(rightTableName, rightFieldName));
        }

        public string CreateConditionFieldNotNull(string tableName, string fieldName)
        {
            return string.Format("(cmsContentXml.xml like '<" + tableName + "%' and  {0} IS NOT NULL )", Field(tableName, fieldName) );
        }
    }
}