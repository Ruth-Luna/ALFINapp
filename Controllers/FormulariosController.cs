using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using ALFINapp.Models;
using ALFINapp.Filters;
using Microsoft.AspNetCore.Mvc;


namespace ALFINapp.Controllers
{
    public class FormularioController : Controller
    {
        public async Task<IActionResult> EnviarFormulario(
            string DNIAsesor,
            string DNICliente,
            string NombreCliente,
            string CelularCliente,
            string AgenciaComercial)
        {
            // Tu lógica actual para insertar datos en la base de datos...

            // Llamar al script de Playwright en C#
            try
            {
                // Crear una instancia del navegador Playwright de manera asíncrona
                var playwright = await Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
                var page = await browser.NewPageAsync();

                // Navegar al formulario de prueba
                await page.GotoAsync("https://forms.office.com/r/UyLj0fGD0R");

                // Completar los campos del formulario con los valores proporcionados

                // 1. Ingresar el DNI del Asesor
                await page.FillAsync("input[name='DNIAsesor']", DNIAsesor);

                // 2. Ingresar el operador (valor fijo: A365)
                await page.FillAsync("input[name='Operador']", "A365");

                // 3. Ingresar el DNI del cliente
                await page.FillAsync("input[name='DNICliente']", DNICliente);

                // 4. Ingresar el nombre del cliente
                await page.FillAsync("input[name='NombreCliente']", NombreCliente);

                // 5. Ingresar el celular del cliente
                await page.FillAsync("input[name='CelularCliente']", CelularCliente);

                // 6. Seleccionar la agencia comercial (asumimos que es un campo <select>)
                await page.SelectOptionAsync("select[name='AgenciaComercial']", AgenciaComercial);

                // 7. Ingresar la acción a realizar (valor fijo: Derivacion)
                await page.FillAsync("input[name='Accion']", "Derivacion");

                // Enviar el formulario
                await page.ClickAsync("button[type='submit']");

                // Esperar que el formulario se haya enviado (opcional)
                await page.WaitForNavigationAsync();

                // Cerrar el navegador
                await browser.CloseAsync();

                TempData["Message"] = "Formulario de prueba enviado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MessageError"] = "Error al enviar el formulario de prueba: " + ex.Message;
            }

            return RedirectToAction("Ventas");
        }
    }

}

