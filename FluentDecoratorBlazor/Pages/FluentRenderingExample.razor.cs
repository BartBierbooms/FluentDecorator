using FluentDecorator;
using FluentDecoratorBlazor.FluentRenderingModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentDecoratorBlazor.Pages
{
    partial class FluentRenderingExample : ComponentBase
    {
        private ModelDecorators.WithDecorators<Ship, IModelDecorators<Ship, ExtendedDecorators>> modelDecorationDefinition { get; set; }
        private Dictionary<Guid, IModelDecorators<StockItem, Decorators>> stockItemDecorations { get; set; } = new Dictionary<Guid, IModelDecorators<StockItem, Decorators>>();
        private IModelDecorators<Ship, ExtendedDecorators> modelDecorated { get; set; }
        private Ship ship { get; set; }

        const string classShow = "show-status";
        const string classHide = "hide-status";
        const string InitialStatus = "Disembark not started";
        const string WaitOnCaptainStatus = "Disembark wait on captains approval";
        const string WaitOnSteermanStatus = "Disembark wait on steermans approval";
        const string DisembarkCanStart = "Disembark can start";
        const string DisembarkFinished = "Disembark Ended";

        protected override void OnInitialized()
        {
            base.OnInitialized();
            CreateModel();
            DefineDecorationsOnShip();
            SetDecorationOnStockItems(DefineDecorationsOnStockItem());
        }
        #region Model and Decoration setup
        private void CreateModel()
        {
            ship = new Ship()
            {
                Name = "The herald of the see",
                Status = DisembarkStatus.WaitOnCaptain,
                Captain = new Captain() { Name = "Captain Jo" },
                Steersman = new Steersman() { Name = "Steerman Gerald" },
                Stock = new List<StockItem>(new[] {
                    new StockItem() { Description = "A bicycle", Weight = 12D},
                    new StockItem() { Description = "An elephant", Weight = 12000D},
                    new StockItem() { Description = "a box with coffee cups", Weight = 2D},
                })
            };
        }
        private ModelDecorators.WithDecorators<StockItem, IModelDecorators<StockItem, Decorators>> DefineDecorationsOnStockItem()
        {
            return ModelDecorators.Init<StockItem, Decorators>()
                .OnModelIfThenElse(s => s.Weight > Ship.MaxDisembarkWeight,
                    attr => attr.Enabled = false,
                    attr => attr.Enabled = true);
        }
        private void SetDecorationOnStockItems(ModelDecorators.WithDecorators<StockItem, IModelDecorators<StockItem, Decorators>> stockItemDecorationDefinition)
        {
            foreach (var stockItem in ship.Stock)
            {
                stockItemDecorations.Add(stockItem.Id, stockItemDecorationDefinition(stockItem));
            }
        }
        private void DefineDecorationsOnShip()
        {
            modelDecorationDefinition = ModelDecorators.Init<Ship, ExtendedDecorators>()
                .OnModelIfThenElse(WhenThereIsStock(), EnableDisembark(), DisableDisembark())
                .OnModelPropIfThenElse(s => s.NextInRowToApproveDisembark() is Captain,
                    s => s.Captain,
                    prp => prp.Class = classShow,
                    prp => prp.Class = classHide)
                .OnModelPropIfThenElse(s => s.Status == DisembarkStatus.WaitOnSteerman,
                    s => s.Steersman,
                    prp => prp.Class = classShow,
                    prp => prp.Class = classHide)
                .OnModelPropWhen(s =>
                    s.Status >= DisembarkStatus.DisembarkCanStart
                    && s.Status < DisembarkStatus.DisembarkFinished
                    , s => s.Status
                    , dec => dec.Visible = true);

            modelDecorated = modelDecorationDefinition(ship);
        }
        private Func<Ship, bool> WhenThereIsStock()
        {
            return x => x.Stock.Any();
        }
        private Action<Decorators> DisableDisembark()
        {
            return x =>
            {
                x.Enabled = false;
                x.Visible = true;
            };
        }
        private Action<Decorators> EnableDisembark()
        {
            return x =>
            {
                x.Enabled = true;
                x.Visible = false;
            };
        }

        #endregion

        protected override bool ShouldRender()
        {
            modelDecorated = modelDecorationDefinition(ship);
            return base.ShouldRender();
        }

        #region event handling
        public void DisEmbarkingApprovedBy(Person person)
        {
            if (person is Captain)
            {
                ship.Captain.ApprovedDisembark = true;
            }
            else if (person is Steersman)
            {
                ship.Steersman.ApprovedDisembark = true;
            }
            ship.EvaluateStatus();
            StateHasChanged();
        }
        public async void Disembark(StockItem stockItem)
        {
            ship.Stock.Remove(stockItem);
            stockItemDecorations.Remove(stockItem.Id);
            ship.EvaluateStatus();
            await InvokeAsync(StateHasChanged);
        }
        #endregion
        public string GetStatus
        {
            get
            {
                switch (ship.Status)
                {
                    case DisembarkStatus.Initial:
                        return InitialStatus;
                    case DisembarkStatus.DisembarkFinished:
                        return DisembarkFinished;
                    case DisembarkStatus.DisembarkCanStart:
                        return DisembarkCanStart;
                    case DisembarkStatus.WaitOnCaptain:
                        return WaitOnCaptainStatus;
                    case DisembarkStatus.WaitOnSteerman:
                        return WaitOnSteermanStatus;
                }
                return InitialStatus;
            }
        }

    }
}
