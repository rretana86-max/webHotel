using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebHotel_vesion1._0.Repositories.Interfaces;
using WebHotel_vesion1._0.Models;

namespace WebHotel_vesion1._0.Controllers
{

    [Authorize]
    public class RolesController : Controller
    {
        private readonly IRol _irol;


        public RolesController(IRol irol) {
            _irol = irol;

        }
        // GET: RolesController1
        [Authorize(Roles = "Administrador,Empleado")]
        public async Task<ActionResult> ListarRoles()
        { List<Rol> listRoles = await _irol.GetRols();
            return View(listRoles);
        }




        public ActionResult Create()
        {
            return View();
        }

        // POST: RolesController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< IActionResult> Create(Rol rol)
        {
           await  _irol.CreateRol(rol);
            return RedirectToAction("ListarRoles");
        }

        // GET: RolesController1/Edit/5
        [Authorize(Roles ="Administrador")]
        public async Task< ActionResult> Edit(int id)
        {
            Rol rol = await _irol.SearchRol(id);
            return View(rol);
        }

        // POST: RolesController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Edit(Rol rolUpdate)
        {
            try
            {
               await  _irol.UpdateRol(rolUpdate);


                return RedirectToAction(nameof(ListarRoles));
            }
            catch
            {
                return View();
            }
        }
             public async Task<ActionResult>  VdoterRol(int id){


             Rol rolinfo= await _irol.SearchRol(id);

             if(rolinfo==null){

      return    RedirectToAction("ListarRoles");

             }

             return View(rolinfo);



             }
        // GET: RolesController1/Delete/5
        public async Task<ActionResult> Delete(int id)

        {
           Rol rol = await  _irol.SearchRol(id);
            
            
            return View(rol);
        }

        // POST: RolesController1/Delete/5
        [HttpPost]
    
        public async Task<ActionResult> DeleteUser(int IdRol)
        {
            try
            {
                await _irol.DeleteRol(IdRol);
            }
            catch
            {
               
            }
            return RedirectToAction("ListarRoles");
        }
    }
}
