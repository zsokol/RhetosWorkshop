﻿using Microsoft.AspNetCore.Mvc;
using Rhetos.Processing.DefaultCommands;
using Rhetos.Processing;
using Rhetos;

namespace Bookstore.Service.Controllers
{
    [Route("demo/[action]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly IProcessingEngine processingEngine;
        private readonly IUnitOfWork unitOfWork;

        public DemoController(IRhetosComponent<IProcessingEngine> processingEngine, IRhetosComponent<IUnitOfWork> unitOfWork)
        {
            this.processingEngine = processingEngine.Value;
            this.unitOfWork = unitOfWork.Value;
        }

        [HttpGet]
        public string ReadBooks()
        {
            var readCommandInfo = new ReadCommandInfo { DataSource = "Bookstore.Book", ReadTotalCount = true };
            var result = processingEngine.Execute(readCommandInfo);
            return $"{result.TotalCount} books.";
        }

        [HttpGet]
        public string WriteBook()
        {
            var newBook = new Bookstore.Book { Title = "NewBook" };
            var saveCommandInfo = new SaveEntityCommandInfo { Entity = "Bookstore.Book", DataToInsert = new[] { newBook } };
            processingEngine.Execute(saveCommandInfo);
            unitOfWork.CommitAndClose(); // Commits and closes database transaction.
            return "1 book inserted.";
        }
    }
}
