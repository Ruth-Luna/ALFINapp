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
                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions 
                {
                    Headless = true,
                    Devtools = true // Habilita los logs de DevTools
                });
                var page = await browser.NewPageAsync();

                // Navegar al formulario de prueba
                await page.GotoAsync("https://forms.office.com/r/UyLj0fGD0R");

                // Completar los campos del formulario con los valores proporcionados

                // 1. Ingresar el DNI del Asesor
                await page.FillAsync("input[aria-labelledby='QuestionId_red7efe31020b490b8edb703f5d535d7e QuestionInfo_red7efe31020b490b8edb703f5d535d7e']", DNIAsesor);
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = "C:/Users/santi/Downloads/TESTEO/screenshot_before_fill1.png" });

                // 2. Ingresar el operador (valor fijo: A365)
                await page.FillAsync("input[aria-labelledby='QuestionId_r972e879cbae54f07a4e712b4d2629d6e QuestionInfo_r972e879cbae54f07a4e712b4d2629d6e']", "A365");
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = "C:/Users/santi/Downloads/TESTEO/screenshot_before_fill2.png" });

                // 3. Ingresar el DNI del cliente
                await page.FillAsync("input[aria-labelledby='QuestionId_r99924cac2a1d41ff8545bb4b36a8b2b5 QuestionInfo_r99924cac2a1d41ff8545bb4b36a8b2b5']", DNICliente);
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = "C:/Users/santi/Downloads/TESTEO/screenshot_before_fill3.png" });

                // 4. Ingresar el nombre del cliente
                await page.FillAsync("input[aria-labelledby='QuestionId_r6c5c337ce40846d781b1c4325e32c959 QuestionInfo_r6c5c337ce40846d781b1c4325e32c959']", NombreCliente);
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = "C:/Users/santi/Downloads/TESTEO/screenshot_before_fill4.png" });

                // 5. Ingresar el celular del cliente
                await page.FillAsync("input[aria-labelledby='QuestionId_rd258a5d0f47745368dcfb72d622e3a41 QuestionInfo_rd258a5d0f47745368dcfb72d622e3a41']", CelularCliente);
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = "C:/Users/santi/Downloads/TESTEO/screenshot_before_fill5.png" });

                // 6. Hacer clic en el div para abrir el dropdown
                await page.ClickAsync("div[aria-labelledby='QuestionId_re78f9559e1fd4c88bc8b73c80f551108']");
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = "C:/Users/santi/Downloads/TESTEO/screenshot_before_fill6.png" });

                // Esperar que las opciones sean visibles
                await page.WaitForSelectorAsync("div[aria-expanded='true']");
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = "C:/Users/santi/Downloads/TESTEO/screenshot_before_fill7.png" });

                // 7. Seleccionar la opción correspondiente al valor de AgenciaComercial
                var sucursalLocator = page.Locator($"div[aria-labelledby='QuestionId_re78f9559e1fd4c88bc8b73c80f551108'] div[role='option'] >> text='{AgenciaComercial}'");
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = "C:/Users/santi/Downloads/TESTEO/screenshot_before_fill8.png" });
                await sucursalLocator.ClickAsync();

                // 8. Ingresar la acción a realizar (valor fijo: Derivacion)
                await page.ClickAsync("input[type='radio'][value='Derivacion']");
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = "C:/Users/santi/Downloads/TESTEO/screenshot_before_fill9.png" });

                // Enviar el formulario
                await page.ClickAsync("button[data-automation-id='submitButton']");

                // Esperar que el formulario se haya enviado (opcional)
                await page.WaitForNavigationAsync();

                // Cerrar el navegador
                await browser.CloseAsync();

                TempData["Message"] = "Formulario de prueba enviado correctamente.";
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = ex.Message });
            }

            return RedirectToAction("Inicio");
        }
    }
}