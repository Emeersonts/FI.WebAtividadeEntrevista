using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using System.Web.Security;
using System.Reflection;
using System.Net;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {

        static long _acesso = 0;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Incluir()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ExcluirBeneficiario(long Id)
        {
            BoCliente bo = new BoCliente();
            bo.ExcluirBeneficiario(Id);

            return View("Incluir");
        }

        [HttpGet]
        public ActionResult AlterarBeneficiario(long Id, string Nome)
        {
            BoCliente bo = new BoCliente();
            bo.AlterarBeneficiario(Id, Nome);

            return View("Incluir");
        }

        public ActionResult Beneficiario()
        {
            try
            {
                long IdCliente = _acesso;

                List<Beneficiario> _Beneficiario = new BoCliente().LBeneficiario(IdCliente);

                List<BeneficiarioModel> model = new List<BeneficiarioModel>();

                foreach (var item in _Beneficiario)
                {
                    BeneficiarioModel model1 = new BeneficiarioModel();

                    model1.IdCliente = item.Idcliente;
                    model1.NOME = item.Nome;
                    model1.Cpf = item.CPF;
                    model1.Id = item.Id;

                    model.Add(model1);
                }

                return View("Beneficiario", model);

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
            
        }

        [HttpPost]
        public ActionResult Beneficiario(BeneficiarioModel model)
        {

            BoCliente bo = new BoCliente(); 

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                Response.StatusCode = 0;
                bool VerificaCpf = bo.VerificarBeneficiario(model.Cpf,_acesso);
                if (VerificaCpf == true)
                {
                    Response.StatusCode = 400;
                    return Json(string.Join("", "Já existe este beneficiário p/ este cliente"));
                }
                else
                    Response.StatusCode = 0;
                bool VerificaDigCpf = bo.VerificaDigito(model.Cpf);
                if (VerificaDigCpf == false)
                {
                    Response.StatusCode = 400;
                    return Json(string.Join("", "O CPF. não é válido"));
                }
                else
                {

                    model.Id = bo.IncluirBeneficiario(new Beneficiario()
                    {
                        Idcliente = _acesso,
                        Nome = model.NOME,
                        CPF = model.Cpf
                    });

                    return View("Index");

                }
            }
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                Response.StatusCode = 0;
                bool VerificaCpf = bo.VerificarExistencia(model.Cpf);
                if (VerificaCpf == true)
                {
                    Response.StatusCode = 400;
                    return Json(string.Join("","Já existe um cliente com este CPF."));
                }
                else
                Response.StatusCode = 0;
                bool VerificaDigCpf = bo.VerificaDigito(model.Cpf);
                if (VerificaDigCpf == false)
                {
                    Response.StatusCode = 400;
                    return Json(string.Join("", "O CPF. não é válido"));
                }
                else
                {

                    model.Id = bo.Incluir(new Cliente()
                    {
                        CEP = model.CEP,
                        Cidade = model.Cidade,
                        Email = model.Email,
                        Estado = model.Estado,
                        Logradouro = model.Logradouro,
                        Nacionalidade = model.Nacionalidade,
                        Nome = model.Nome,
                        Sobrenome = model.Sobrenome,
                        Telefone = model.Telefone,
                        Cpf = model.Cpf
                    });


                    return Json("Cadastro efetuado com sucesso");
                }
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
       
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    Cpf = model.Cpf
                });
                               
                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            _acesso = id;

            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);

            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    Cpf = cliente.Cpf
                };

            
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        [HttpGet]
        public JsonResult BeneficiarioList(long IdCliente)
        {
            try
            {
                IdCliente = _acesso;

                List<Beneficiario> _beneficiarios = new BoCliente().LBeneficiario(IdCliente);

                return Json(new { Result = "OK", Records = _beneficiarios });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

    }
}