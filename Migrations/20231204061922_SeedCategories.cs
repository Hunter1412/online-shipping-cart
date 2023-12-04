using Microsoft.EntityFrameworkCore.Migrations;
using OnlineShoppingCart.Data.Entities;

#nullable disable

namespace OnlineShoppingCart.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData("Categories",
                columns: new[] {
                    nameof(Category.Id),
                    nameof(Category.Name),
                    nameof(Category.Slug),
                    nameof(Category.ParentId),
                    nameof(Category.CreateAt)
                },
                values: new object[,] {
                    {"CP10000","Stationery","Stationery-CP10000",null,DateTime.Now},
                    {"CP10100","NoteBooks","NoteBooks-CP10100","CP10000",DateTime.Now},
                    {"CP10101","Ruled","Ruled-CP10101","CP10100",DateTime.Now},
                    {"CP10102","Plain","Plain-CP10102","CP10100",DateTime.Now},
                    {"CP10200","Planners","Planners-CP10200","CP10000",DateTime.Now},
                    {"CP10201","Journals","Journals-CP10201","CP10200",DateTime.Now},
                    {"CP10202","Calendars","Calendars-CP10202","CP10200",DateTime.Now},
                    {"CP10300","Pen","Pen-CP10300","CP10000",DateTime.Now},
                    {"CP10301","Gel-Pens","Gel-Pens-CP10301","CP10300",DateTime.Now},
                    {"CP10302","Ball-Pens","Ball-Pens-CP10302","CP10300",DateTime.Now},
                    {"CP10303","Rollerball-Pens","Rollerball-Pens-CP10303","CP10300",DateTime.Now},
                    {"CP10304","Fountain-Pens","Fountain-Pens-CP10304","CP10300",DateTime.Now},
                    {"CP10305","Inks-and-Cartridges","Inks-and-Cartridges-CP10305","CP10300",DateTime.Now},
                    {"CP10400","Pencils","Pencils-CP10400","CP10000",DateTime.Now},
                    {"CP10401","HB--Pencils","HB--Pencils-CP10401","CP10400",DateTime.Now},
                    {"CP10402","Mechanical-Pencils","Mechanical-Pencils-CP10402","CP10400",DateTime.Now},
                    {"CP10403","Pencil-Cases-and-Pouches","Pencil-Cases-and-Pouches-CP10403","CP10400",DateTime.Now},
                    {"CP10500","Sharpeners","Sharpeners-CP10500","CP10000",DateTime.Now},
                    {"CP10600","Eraser-and-Correction","Eraser-and-Correction-CP10600","CP10000",DateTime.Now},
                    {"CP10700","Highlighters","Highlighters-CP10700","CP10000",DateTime.Now},
                    {"CP10800","Notepads","Notepads-CP10800","CP10000",DateTime.Now},
                    {"CP10900","Sticky-Notes","Sticky-Notes-CP10900","CP10000",DateTime.Now},
                    {"CP11000","Rulers-and-Measuring-Tools","Rulers-and-Measuring-Tools-CP11000","CP10000",DateTime.Now},
                    {"CP20000","Office-Supplies","Office-Supplies-CP20000",null,DateTime.Now},
                    {"CP20100","Calculators","Calculators-CP20100","CP20000",DateTime.Now},
                    {"CP20200","Staplers-and-Pins","Staplers-and-Pins-CP20200","CP20000",DateTime.Now},
                    {"CP20300","Cutters","Cutters-CP20300","CP20000",DateTime.Now},
                    {"CP20400","Pen-Stand-and-Organisers","Pen-Stand-and-Organisers-CP20400","CP20000",DateTime.Now},
                    {"CP20500","Paperclips-Fasteners-and-Rubber-bands","Paperclips-Fasteners-and-Rubber-bands-CP20500","CP20000",DateTime.Now},
                    {"CP20600","Folder-and-Filling","Folder-and-Filling-CP20600","CP20000",DateTime.Now},
                    {"CP20700","Punches","Punches-CP20700","CP20000",DateTime.Now},
                    {"CP20800","White-board-Markets","White-board-Markets-CP20800","CP20000",DateTime.Now},
                    {"CP20900","Scissors-and-Paper-Trimmers","Scissors-and-Paper-Trimmers-CP20900","CP20000",DateTime.Now},
                    {"CP30000","Art-Supplies","Art-Supplies-CP30000",null,DateTime.Now},
                    {"CP30100","Paints","Paints-CP30100","CP30000",DateTime.Now},
                    {"CP30200","Art-Pencils","Art-Pencils-CP30200","CP30000",DateTime.Now},
                    {"CP30300","Crayons-and-Oil-Pastels","Crayons-and-Oil-Pastels-CP30300","CP30000",DateTime.Now},
                    {"CP30400","Artist-Sketch-Pads-and-Sheets","Artist-Sketch-Pads-and-Sheets-CP30400","CP30000",DateTime.Now},
                    {"CP30500","Drawing-Books","Drawing-Books-CP30500","CP30000",DateTime.Now},
                    {"CP30101","Water-Colour","Water-Colour-CP30101","CP30100",DateTime.Now},
                    {"CP30102","Acrylic-Colour","Acrylic-Colour-CP30102","CP30100",DateTime.Now},
                    {"CP30103","Poster-Colour","Poster-Colour-CP30103","CP30100",DateTime.Now},
                    {"CP30201","Coloured-pencils","Coloured-pencils-CP30201","CP30200",DateTime.Now},
                    {"CP30202","Water-Soluble-pencils","Water-Soluble-pencils-CP30202","CP30200",DateTime.Now},
                    {"CP30203","Sketch-pencils","Sketch-pencils-CP30203","CP30200",DateTime.Now},
                    {"CP30204","Charcoal-pencils","Charcoal-pencils-CP30204","CP30200",DateTime.Now},
                    {"CP30301","Oil-Pastels","Oil-Pastels-CP30301","CP30300",DateTime.Now},
                    {"CP30302","Wax-Crayons","Wax-Crayons-CP30302","CP30300",DateTime.Now},
                    {"CP30401","Sketch-and-Drawing","Sketch-and-Drawing-CP30401","CP30400",DateTime.Now},
                    {"CP30402","Watercolour-Pads-and-Sheets","Watercolour-Pads-and-Sheets-CP30402","CP30400",DateTime.Now},
                    {"CP30403","Acrylic-Pads-and-Sheets","Acrylic-Pads-and-Sheets-CP30403","CP30400",DateTime.Now},
                    {"CP30404","Oil-Painting-Pads-and-Sheets","Oil-Painting-Pads-and-Sheets-CP30404","CP30400",DateTime.Now},
                    {"CP30405","Loose-Sheets","Loose-Sheets-CP30405","CP30400",DateTime.Now},
                    {"CP30501","Mandala","Mandala-CP30501","CP30500",DateTime.Now},
                    {"CP30502","Artist-Drawing-Books","Artist-Drawing-Books-CP30502","CP30500",DateTime.Now},
                    {"CP40000","Craft-Material","Craft-Material-CP40000",null,DateTime.Now},
                    {"CP40100","Masking-and-Decoration-Tapes","Masking-and-Decoration-Tapes-CP40100","CP40000",DateTime.Now},
                    {"CP40200","Glue-and-Adhesives","Glue-and-Adhesives-CP40200","CP40000",DateTime.Now},
                    {"CP40300","Stencils-and-Stickers","Stencils-and-Stickers-CP40300","CP40000",DateTime.Now},
                    {"CP40400","Stamps-and-Pads","Stamps-and-Pads-CP40400","CP40000",DateTime.Now},
                    {"CP40500","Backpacks","Backpacks-CP40500","CP40000",DateTime.Now},
                    {"CP40600","Glue-and-Adhesives","Glue-and-Adhesives-CP40600","CP40000",DateTime.Now},
                    {"CP40700","Stencils-and-Stickers","Stencils-and-Stickers-CP40700","CP40000",DateTime.Now},
                    {"CP40800","Stamps-and-Pads","Stamps-and-Pads-CP40800","CP40000",DateTime.Now},
                    {"CP40900","Backpacks","Backpacks-CP40900","CP40000",DateTime.Now}
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Categories", true);
        }
    }
}
