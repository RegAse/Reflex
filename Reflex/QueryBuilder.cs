using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflex
{
    public class QueryBuilder
    {
        private OrderBy _orderby { get; set; }
        private List<Where> _where { get; set; }

        public QueryBuilder()
        {
            _where = new List<Where>();
        }

        public T First<T>()
        {
            throw new NotImplementedException();
        }

        public List<T> Get<T>()
        {
            return Reflex.Sele<T>(_where);
        }

        public QueryBuilder OrderBy(string column, string order)
        {
            _orderby = new OrderBy(column, order);
            return this;
        }

        public QueryBuilder Where(string column, string oper, string value)
        {
            _where.Add(new Where(column,oper,value));
            return this;
        }

    }

    public class OrderBy
    {
        public string column { get; set; }
        public string order { get; set; }
        
        public OrderBy(string _column, string _order)
        {
            column = _column;
            order = _order;
        }
    }

    public class Where
    {
        public string column { get; set; }
        public string oper { get; set; }
        public string value { get; set; }

        public Where(string _column, string _oper, string _value)
        {
            column = _column;
            oper = _oper;
            value = _value;
        }
    }
}
