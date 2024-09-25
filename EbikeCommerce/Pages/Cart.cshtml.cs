using EbikeCommerce.DBmodel;
using EbikeCommerce.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeCommerce.Pages
{
    public class CartModel : PageModel
    {
        public List<ProductRecord> rec { get; set; } = new List<ProductRecord>();

        [BindProperty]
        public string? Message { get; set; }

        public IActionResult OnGet()
        {
            rec.AddRange(GetRecordsFromSessionList()!);
            return Page();
        }

        public IActionResult OnPostCheckout()
        {
            List<int>? carrello = HttpContext.Session.GetObject<List<int>>($"Carrello-{User.Identity!.Name}");

            if (User?.Identity?.IsAuthenticated == false)
            {
                return RedirectToPage("/Login", new { message = "You must LogIn before buy" });
            }
            if (carrello != null && carrello.Any())
            {
                return RedirectToPage("/Checkout");
            }
            else
            {
                Message = "Il carrello è vuoto, aggiungi un prodotto prima di acquistare";
                rec.AddRange(GetRecordsFromSessionList()!);
                return Page();
            }
        }

        public IActionResult OnPost(int id)
        {
            int qta;
            List<int>? carrello = HttpContext.Session.GetObject<List<int>>($"Carrello-{User.Identity!.Name}") ?? new List<int>();

            if (carrello.Contains(id))
                qta = carrello.Count(x => x == id);
            else
                qta = 1;

            if (DBservice.CheckStocks(id, qta))
            {
                Add(id);
                return Page();
            }
            else
            {
                Message = $"Non puoi più aggiungere {DBservice.GetbyID(id)?.product_name} . Stock non disponibile";
                rec.AddRange(GetRecordsFromSessionList()!);
                return Page();
            }
        }

        public IActionResult OnPostRemove(int id)
        {
            List<int> carrello = HttpContext.Session.GetObject<List<int>>($"Carrello-{User.Identity!.Name}") ?? new List<int>();
            carrello.Remove(id);

            HttpContext.Session.SetObject($"Carrello-{User.Identity!.Name}", carrello);
            return RedirectToPage();
        }

        public void Add(int idProdotto)
        {
            // Recupera la lista del carrello dalla sessione
            List<int> carrello = HttpContext.Session.GetObject<List<int>>($"Carrello-{User.Identity!.Name}") ?? new List<int>();

            // Aggiungi l'ID del prodotto al carrello
            carrello.Add(idProdotto);

            // Salva la lista del carrello nella sessione
            HttpContext.Session.SetObject($"Carrello-{User.Identity!.Name}", carrello);

            rec.AddRange(GetRecordsFromSessionList()!);
        }

        private List<ProductRecord?> GetRecordsFromSessionList()
        {
            List<int> carrello = HttpContext.Session.GetObject<List<int>>($"Carrello-{User.Identity!.Name}") ?? new List<int>();

            return carrello
                .Select(DBservice.GetbyID)
                .Where(record => record != null)
                .ToList();
        }
    }
}