﻿using ArticlesPOSTGREDBCRUDOperations.Data;
using ArticlesPOSTGREDBCRUDOperations.DTOs;
using ArticlesPOSTGREDBCRUDOperations.Models;
using ArticlesPOSTGREDBCRUDOperations.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArticlesPOSTGREDBCRUDOperations.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppDBContext _context;
        private readonly ArticleService _articleService;

        public ArticlesController(IMapper mapper, AppDBContext context, ArticleService articleService)
        {            
            _mapper = mapper;
            _context = context;
            _articleService = articleService;
        }

        [HttpGet]
        // GET: api/articles
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles()
        {            
            var articles = await _context.Articles.ToListAsync();
            var articleDtos = _mapper.Map<List<ArticleDto>>(articles);
            return Ok(articleDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDto>> GetArticle(int id)
        {            
            var article = await _articleService.GetArticleWithAuthor(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        [HttpPost]
        public async Task<ActionResult<Article>> PostArticle(Article article)
        {
            article.CreatedAt = DateTime.UtcNow;
            _context.Articles.Add(article);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticle", article.Id, article);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, Article article)
        {
            if (id != article.Id)
            {
                return BadRequest();
            }
            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {            
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(a => a.Id == id);
        }
    }
}
