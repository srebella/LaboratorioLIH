using System;
using System.Collections.Generic;
using System.Linq;
using laberegisterLIH.Data;
using laberegisterLIH.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class DbInitializer : IDbInitializer {
        private readonly IServiceScopeFactory _scopeFactory;

        public DbInitializer (IServiceScopeFactory scopeFactory) {
            this._scopeFactory = scopeFactory;
        }

        public void Initialize () {
            using (var serviceScope = _scopeFactory.CreateScope ()) {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext> ()) {
                    context.Database.Migrate();
                }
            }
        }

        public void SeedData () {
            using (var serviceScope = _scopeFactory.CreateScope ()) {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext> ()) {

                    context.Clientes.Add(new Cliente(){
                        Firstname ="Santiago",
                        Lastname = "Rebella",
                        UserName = "santi.rebella87@gmail.com",
                        Email = "santi.rebella87@gmail.com",
                        Telephone = "123123123",
                        Address = "123123123",
                        CreatedOn = DateTime.UtcNow,
                        PasswordHash = "",

                    });    
                    //add exams
                    if (!context.Examenes.Any ()) {
                        var examenesList= new List<Examen>(){
                            new Examen(){
                               
                                Name = "CORTISOL PRE Y POST DEXAMETASONA macrodosis",
                                Requirements = "AYUNO:"+
                                        "* Presentarse en el laboratorio en los horarios de toma de muestra: de 7"+

                                        "* El ayuno no debe ser mayor de 12 horas ni menor de 10 horas. La noche anterior a su examen ingerir la última comida entre 7 y 8 pm, idealmente libre de grasas."+

                                        "* No ingiera bebidas alcohólicas antes de 72 horas."+

                                        "* Prohibido fumar antes o durante sus exámenes de laboratorio."+

                                        "* Evitar Deporte."+

                                        "* Informarle al personal del laboratorio si su médico hizo alguna recomendación para la toma de muestra, si toma algún medicamento (nombre y dosis) o si viene de realizar una actividad que demande esfuerzo físico (debe permanecer en reposo 30 minutos). No debe tomar sus medicamentos antes de tomar sus muestras. No se preocupe, puede tomarlos inmediatamente obtengan sus muestras.",
                                Price = "1",
                                Protocols = "Formulario 1",
                                Tags = "CORTISOL PRE Y POST DEXAMETASONA macrodosis",//cortar en split tags
                                CreatedOn = DateTime.UtcNow            
                            },
                       
                        
                            new Examen(){
                                Name = "11 DEOXICORTISOL",
                                Requirements = "AYUNO:"+
                                            "* Presentarse en el laboratorio en ayuno, en los horarios de toma de muestra de 7 a 10 am."+
                                            "* El ayuno no debe ser mayor de 12 horas ni menor de 10 horas. La noche anterior a su examen ingerir la última"+
                                            "comida entre 7 y 8 pm, idealmente libre de grasas."+
                                            "* No ingiera bebidas alcohólicas antes de 72 horas."+
                                            "* Prohibido fumar antes o durante sus exámenes de laboratorio."+
                                            "* Evitar Deporte."+
                                            "* Informarle al personal del laboratorio si su médico hizo alguna recomendación para la toma de muestra, si"+
                                            "toma algún medicamento (nombre y dosis) o si viene de realizar una actividad que demande esfuerzo físico (debe"+
                                            "permanecer en reposo 30 minutos). No debe tomar sus medicamentos antes de tomar sus muestras. No se"+
                                            "preocupe, puede tomarlos inmediatamente obtengan sus muestras",
                                Price = "1",
                                Protocols = "Formulario 2",
                                Tags = "11 DEOXICORTISOL, 11 - DESOXICORTISOL, CORTICOSTERONA, DESOXICORTISOL, 11 - DEOXICORTICOSTEROIDE",//cortar en split tags
                                CreatedOn = DateTime.UtcNow            
                            },
                            new Examen(){
                                Name = "17 HIDROXI CORTICOSTEROIDES",
                                Requirements = "ORINA 24 HORAS:"+
                                    "1. Solicitar galón para recolección."+
                                    "2. Comience la recolección de orina en la mañana."+
                                    "3. Desocupe la vejiga cuando se levante y deseche esta primera orina. Anote la hora."+
                                    "4. En el galón obtenido en el laboratorio, recolecte toda la orina que produzca durante el día y la noche (24 horas) incluyendo la primera orina del día siguiente que se deberá recolectar a la misma hora del día anterior. Conserve la orina recolectada en un lugar fresco o refrigere en la nevera si es posible durante las 24 horas."+
                                    "5. Informarle al personal del laboratorio si su médico hizo alguna recomendación para la toma de muestra, si toma algún medicamento (nombre y dosis) o si viene de realizar una actividad que demande esfuerzo físico (debe permanecer en reposo 30 minutos). No debe tomar sus medicamentos antes de tomar sus muestras. No se preocupe, puede tomarlos inmediatamente obtengan sus muestras."+
                                    "TENGA EN CUENTA:"+
                                    "* No debe recoger la orina si esta con la menstruación; deje pasar tres días y luego recoja la muestra."+
                                    "* Traiga el galón con toda la orina recolectada lo más pronto posible al laboratorio."+
                                    "* No ingerir bebidas alcohólicas, ni cantidades excesivas de agua",
                                Price = "1",
                                Protocols = "Formulario 3",
                                Tags = "17 HIDROXI CORTICOSTEROIDES, 17-OHCS, CROMÓGENO PORTER-SILVER",//cortar en split tags
                                CreatedOn = DateTime.UtcNow            
                            },
                            new Examen(){
                                Name = "ADIPONECTINA",
                                Requirements = "AYUNO:"+
                                            "* Presentarse en el laboratorio en ayuno, en los horarios de toma de muestra de 7 a 10 am."+
                                            "* El ayuno no debe ser mayor de 12 horas ni menor de 10 horas. La noche anterior a su examen ingerir la última"+
                                            "comida entre 7 y 8 pm, idealmente libre de grasas."+
                                            "* No ingiera bebidas alcohólicas antes de 72 horas."+
                                            "* Prohibido fumar antes o durante sus exámenes de laboratorio."+
                                            "* Evitar Deporte."+
                                            "* Informarle al personal del laboratorio si su médico hizo alguna recomendación para la toma de muestra, si"+
                                            "toma algún medicamento (nombre y dosis) o si viene de realizar una actividad que demande esfuerzo físico (debe"+
                                            "permanecer en reposo 30 minutos). No debe tomar sus medicamentos antes de tomar sus muestras. No se"+
                                            "preocupe, puede tomarlos inmediatamente obtengan sus muestras",
                                Price = "1",
                                Protocols = "Formulario 4",
                                Tags = "ADIPONECTINA, ACRP 30, GBP 28, ADIPONECTIN 30, ADIPO Q, APM 1",//cortar en split tags
                                CreatedOn = DateTime.UtcNow            
                            },
                            new Examen(){
                                Name = "21 HIDROXILASA ANTICUERPOS",
                                Requirements = "AYUNO:"+
                                        "* Presentarse en el laboratorio en ayuno, en los horarios de toma de muestra de 7 a 10 am."+
                                        "* El ayuno no debe ser mayor de 12 horas ni menor de 10 horas. La noche anterior a su examen ingerir la última"+
                                        "comida entre 7 y 8 pm, idealmente libre de grasas."+
                                        "* No ingiera bebidas alcohólicas antes de 72 horas."+
                                        "* Prohibido fumar antes o durante sus exámenes de laboratorio."+
                                        "* Evitar Deporte."+
                                        "* Informarle al personal del laboratorio si su médico hizo alguna recomendación para la toma de muestra, si"+
                                        "toma algún medicamento (nombre y dosis) o si viene de realizar una actividad que demande esfuerzo físico (debe"+
                                        "permanecer en reposo 30 minutos). No debe tomar sus medicamentos antes de tomar sus muestras. No se"+
                                        "preocupe, puede tomarlos inmediatamente obtengan sus muestras.",
                                Price = "1",
                                Protocols = "Formulario 5",
                                Tags = "21 HIDROXILASA ANTICUERPOS",//cortar en split tags
                                CreatedOn = DateTime.UtcNow            
                            },
                            new Examen(){
                                Name = "ACIDO FOLICO",
                                Requirements = "AYUNO:"+
                                "* Presentarse en el laboratorio en ayuno, en los horarios de toma de muestra de 7 a 10 am."+
                                "* El ayuno no debe ser mayor de 12 horas ni menor de 10 horas. La noche anterior a su examen ingerir la última comida entre 7 y 8 pm, idealmente libre de grasas."+
                                "* No ingiera bebidas alcohólicas antes de 72 horas."+
                                "* Prohibido fumar antes o durante sus exámenes de laboratorio."+
                                "* Evitar Deporte."+
                                "* Informarle al personal del laboratorio si su médico hizo alguna recomendación para la toma de muestra, si toma algún medicamento (nombre y dosis) o si viene de realizar una actividad que demande esfuerzo físico (debe permanecer en reposo 30 minutos). No debe tomar sus medicamentos antes de tomar sus muestras. No se preocupe, puede tomarlos inmediatamente obtengan sus muestras.",
                                Price = "1",
                                Protocols = "Formulario 6",
                                Tags = "ACIDO FOLICO, FOLATO, N5-METIL-TETRAHIDROFOLATO, VITAMINA B9",//cortar en split tags
                                CreatedOn = DateTime.UtcNow            
                            },
                            new Examen(){
                                Name = "ACIDO HOMOVANILICO",
                                Requirements = "* Evitar alcohol, café, té, tabaco y ejercicio fuerte antes y durante la recolección"+
                                "ORINA 24 HORAS Y PARCIAL: Según orden médica:"+
                                "ORINA 24 HORAS:"+
                                "1. Solicitar galón para recolección."+
                                "2. Comience la recolección de orina en la mañana."+
                                "3. Desocupe la vejiga cuando se levante y deseche esta primera orina. Anote la hora."+
                                "4. En el galón obtenido en el laboratorio, recolecte toda la orina que produzca durante el día y la noche (24 horas) incluyendo la primera orina del día siguiente que se deberá recolectar a la misma hora del día anterior. Conserve la orina recolectada en un lugar fresco o refrigere en la nevera si es posible durante las 24 horas."+
                                "5. Informarle al personal del laboratorio si su médico hizo alguna recomendación para la toma de muestra, si toma algún medicamento (nombre y dosis) o si viene de realizar una actividad que demande esfuerzo físico (debe permanecer en reposo 30 minutos). No debe tomar sus medicamentos antes de tomar sus muestras. No se preocupe, puede tomarlos inmediatamente obtengan sus muestras."+
                                "ORINA PARCIAL:"+
                                "1. Obtener frasco estéril para recolectar orina."+
                                "2. Debe abrir el frasco de orina únicamente en el momento de recoger la muestra."+
                                "3. Lavar el área genital externa con agua y jabón."+
                                "4. Separar los labios (para mujeres), y recoger la segunda parte del chorro."+
                                "5. Realizar limpieza del Glande (para hombres), y recoger la segunda parte del chorro."+
                                "6. Cerrar el frasco inmediatamente."+
                                "7. Marcar el frasco con nombre y fecha."+
                                "8. Llevar la muestra antes de dos (2) horas al laboratorio."+

                                "TENGA EN CUENTA:"+
                                "* No debe recoger la orina si esta con la menstruación; deje pasar tres días y luego recoja la muestra."+
                                "* Traiga el galón con toda la orina recolectada lo más pronto posible al laboratorio."+
                                "* Para niños menores de dos años, utilizar bolsa pediátrica y evitar contaminación con materia fecal."+
                                "* No ingerir bebidas alcohólicas, ni cantidades excesivas de agua.",
                                Price = "1",
                                Protocols = "Formulario 7",
                                Tags = "ACIDO HOMOVANILICO, HVA",//cortar en split tags
                                CreatedOn = DateTime.UtcNow            
                            },
                            new Examen(){
                                Name = "ACIDO LACTICO",
                                Requirements = "AYUNO:"+
                                            "* Presentarse en el laboratorio en ayuno, en los horarios de toma de muestra de 7 a 10 am."+
                                            "* El ayuno no debe ser mayor de 12 horas ni menor de 10 horas. La noche anterior a su examen ingerir la última"+
                                            "comida entre 7 y 8 pm, idealmente libre de grasas."+
                                            "* No ingiera bebidas alcohólicas antes de 72 horas."+
                                            "* Prohibido fumar antes o durante sus exámenes de laboratorio."+
                                            "* Evitar Deporte."+
                                            "* Informarle al personal del laboratorio si su médico hizo alguna recomendación para la toma de muestra, si"+
                                            "toma algún medicamento (nombre y dosis) o si viene de realizar una actividad que demande esfuerzo físico (debe"+
                                            "permanecer en reposo 30 minutos). No debe tomar sus medicamentos antes de tomar sus muestras. No se"+
                                            "preocupe, puede tomarlos inmediatamente obtengan sus muestras",
                                Price = "1",
                                Protocols = "Formulario 8",
                                Tags = "ACIDO LACTICO, L-LACTATO, LACTATO EN SANGRE, ACIDO 2-HIDROXIi-PROPANOICO; ALFA-HIDROXI-PROPANOICO",//cortar en split tags
                                CreatedOn = DateTime.UtcNow            
                            },
                            new Examen(){
                                Name = "ACIDO VALPROICO",
                                Requirements = "* Relacionar dosis, fecha y hora de la última dosis, fecha y hora de la toma de la muestra, vía de administración y diagnóstico del paciente."+
                                "* La muestra debe ser tomada de 8-12 Horas después de la última dosis del medicamento."+
                                "* No requiere ayuno.",
                                Price = "1",
                                Protocols = "Formulario 9",
                                Tags = "ACIDO VALPROICO, ACIDO DIPROFILACETICO, DEPACON, DEPAKENE, DEPAKOTE, DEPAMIDE, VALPROATO DE SODIO, VALPROATO SEMISODIO, VALCOTE, LEPTILAN",//cortar en split tags
                                CreatedOn = DateTime.UtcNow            
                            },
                            new Examen(){
                                Name = "ACTINA ANTICUERPOS SUERO",
                                Requirements = "AYUNO: No requiere"+

                                "* Presentarse en el laboratorio, en los horarios de toma de muestra de 7 a 10 am."+

                                "* Libre de hemólisis, lipemia e ictericia."+

                                "* Informarle al personal del laboratorio si su médico hizo alguna recomendación para la toma de muestra, si toma algún medicamento (nombre y dosis)",
                                Price = "1",
                                Protocols = "Formulario 10",
                                Tags = "ACTINA ANTICUERPOS SUERO",//cortar en split tags
                                CreatedOn = DateTime.UtcNow            
                            }
                        };
                        foreach (var examen in examenesList)
                        {                               
                            context.Examenes.Add(examen);
                        }
                    }
                    var sucursalesList= new List<Sucursal>(){
                        new Sucursal(){
                            Name="Bosque",
                            Telephone="1234-5678",
                            Address ="Cl. 134 #7b-83",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Centro Internacional",
                            Telephone="1234-5678",
                            Address ="Carrera 13-A No. 34-55 Cons. 101 - 102 Ext. 317",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Chia",
                            Telephone="1234-5678",
                            Address ="Av. Pradilla No. 5 - 31 ESTE Local 127 CC Plaza Mayor Bogotá",
                            PostalCode = "M1234",
                            City = "Chia",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Chico",
                            Telephone="1234-5678",
                            Address ="Carrera 10 No. 96-25 Consultorio 205 Ext. 311",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Country",
                            Telephone="1234-5678",
                            Address ="Carrera 16 No. 84-A -09 Consultorio 613 Ext. 305",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Country Cima",
                            Telephone="1234-5678",
                            Address ="Calle 83 No. 16-A - 44 Consultorio 208 Ext. 327",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Country Medical Center",
                            Telephone="1234-5678",
                            Address ="Carrera. 19 A No. 82-85 Consultorio 223",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Marly",
                            Telephone="1234-5678",
                            Address ="Calle 50 No. 8-24 Consultorio 101 Ext. 307",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Pasadena",
                            Telephone="1234-5678",
                            Address ="Calle 106 #56- 41 Piso 2 Ext. 313",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Salitre",
                            Telephone="1234-5678",
                            Address ="Av. Calle 24 No. 69c - 17 Junto A Metro Express",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Santa Barbara",
                            Telephone="1234-5678",
                            Address ="Carrera 7 No. 121-95 Ext 201",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Sede Americas",
                            Telephone="1234-5678",
                            Address ="Calle 6A No. 70 - 26",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Toberin",
                            Telephone="1234-5678",
                            Address ="Carrera 21 No. 166 - 81",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Sucursal(){
                            Name="Unicentro",
                            Telephone="1234-5678",
                            Address ="Cll 127 # 14 - 12",
                            PostalCode = "M1234",
                            City = "Bogotá",
                            OpeningTime = "6:00",
                            ClosingTime = "12:00",
                            WorkingDays = "L a Sab",
                            CreatedOn = DateTime.UtcNow
                        }
                    };
                    foreach (var sucursal in sucursalesList)
                    {                           
                        context.Sucursales.Add(sucursal);
                    }
                    context.SaveChanges();
                }
            }
        }
    }