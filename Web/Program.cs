using Web.Utilities.AutoMapper;
using LayerControllerInversion;
using Web.Utilities.Extensiones;
using DinkToPdf;
using DinkToPdf.Contracts;

using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//configirando cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
    AddCookie(option =>
    {
        //indicando cual es el formulario de logue
        option.LoginPath = "/Login/Acceso";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);//em 20 minutos expira la sesion del usuario
    });


builder.Services.AddRazorPages().AddRazorRuntimeCompilation();//para poder realizar cambios en tiempo de ejecucion sin reiniciar el servidor
builder.Services.DependencyInyection(builder.Configuration);//configuracion a la capa de inyeccion
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));//inyectando configuracion de mapeo


var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "Utilities/PdfLibrary/libwkhtmltox.dll"));//obteniendo el directiorio del proyecto

builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));//sincronizando el conventidor a pdf para poderlo crear

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Acceso}/{id?}");

app.Run();
