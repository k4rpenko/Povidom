using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGAdminDAL.Model
{
    public class Сoments
    {
        public string PostId { get; set; }
        public List<string> CommentId { get; set; } = new List<string>();
    }
}
