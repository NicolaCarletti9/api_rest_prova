﻿using apiRestProva.Db;
using apiRestProva.Entities;
using apiRestProva.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
namespace apiRestProva.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClientService httpClient;
        private readonly ProvaDbContext dbContext;

        public CartService(HttpClientService _httpClient, ProvaDbContext _dbContext)
        {
            httpClient = _httpClient;
            dbContext = _dbContext;
        }



        public Task<string> Buy(string cartId, decimal totalAmount)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateCart(string username, string deviceId)
        {
            var cart = new Cart
            {
                
                articles = new List<ArticleCart>(),
                expireCartDatetime = DateTime.UtcNow.AddMinutes(10),
                cartId = deviceId + DateTime.UtcNow.ToString().Trim(),
                totalAmount = 0
            };
            
            await dbContext.AddCart(cart);
            return cart.cartId;

        }

        public async Task<List<ArticleDTO>> GetArticles()
        {

            HttpResponseMessage response = await httpClient.Get("Articoli");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ArticleDTO>>(jsonString);

            }
            return null;
        }
        public Task<CartDTO> GetCart(string cartId)
        {
            var cart = dbContext.Carts.Include(c => c.articles).FirstOrDefault(c => c.cartId == cartId);
            return Task.FromResult(cart.MapToDTO());
        }

        public Task<Cart> Preview(string cartId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateCart(string cartId, ArticleCartDTO article)
        {
            //chiedi samu gestione articoli
            var aCart = new ArticleCart
            {
                CartId = cartId,
                ArticleCode = article.ArticleCode,
                Price = article.Price,
                Quantity = article.Quantity
            };
            //gestione await chiedi samu
            var cart = await dbContext.Carts.Include(c=>c.articles).FirstOrDefaultAsync(c=>c.cartId == cartId);
            cart.articles.Add(aCart);
       
            await dbContext.AddArticleCart(aCart);
            dbContext.SaveChanges();
        }
    }
}
