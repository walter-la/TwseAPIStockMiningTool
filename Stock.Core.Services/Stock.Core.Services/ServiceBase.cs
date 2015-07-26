using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stock.Core.Domain.Models;
using Stock.Core.Services.Interfaces;

namespace Stock.Core.Services
{
    public abstract class ServiceBase :ICompaniesInfo, IDisposable
    {
        internal Entities _context = new Entities();

        public IList<int> GetAllStockID()
        {
            return _context.Companies.Select(c => c.StockID).OrderBy(id => id).ToList();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
