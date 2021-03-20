using System;
using System.Globalization;
using System.Linq;
using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;

namespace EducNotes.API.Helpers {
    public class AutoMapperProfiles : Profile {
        private readonly DataContext _context;
        CultureInfo frC = new CultureInfo ("fr-FR");

        public AutoMapperProfiles (DataContext context) {
          _context = context;
        }
        public AutoMapperProfiles () {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                });
            CreateMap<User, UserForAutoCompleteDto>()
                .ForMember(dest => dest.ClassName, opt => {
                    opt.MapFrom(src => src.Class.Name);
                })
                .ForMember(dest => dest.UserTypeName, opt => {
                    opt.MapFrom(src => src.UserType.Name);
                });

            CreateMap<User, UserForCallSheetDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                });
            CreateMap<Schedule, TeacherClassSessionDto>()
                .ForMember(dest => dest.TeacherName, opt => {
                  opt.MapFrom(src => src.Teacher.LastName + ' ' + src.Teacher.FirstName);
                })
                .ForMember(dest => dest.CourseName, opt => {
                  opt.MapFrom(d => d.Course.Name);
                })
                .ForMember(dest => dest.ClassName, opt => {
                    opt.MapFrom(d => d.Class.Name);
                })
                .ForMember(dest => dest.CourseStartHM, opt => {
                    opt.MapFrom(src => src.StartHourMin);
                })
                .ForMember(dest => dest.CourseEndHM, opt => {
                    opt.MapFrom(src => src.EndHourMin);
                })
                .ForMember(dest => dest.ScheduleId, opt => {
                    opt.MapFrom(src => src.Id);
                })
                .ForMember(dest => dest.StartHourMin, opt => {
                    opt.MapFrom(src => src.StartHourMin.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.EndHourMin, opt => {
                    opt.MapFrom(src => src.EndHourMin.ToString("HH:mm", frC));
                });
            CreateMap<User, UserForClassAllocationDto>()
                .ForMember(dest => dest.LastName, opt => {
                    opt.MapFrom(src => src.LastName);
                })
                .ForMember(dest => dest.FirstName, opt => {
                    opt.MapFrom(d => d.FirstName.ToUpper().First() + d.FirstName.Substring(1));
                })
                .ForMember(dest => dest.Gender, opt => {
                    opt.MapFrom(d => d.Gender);
                })
                .ForMember(dest => dest.DateOfBirth, opt => {
                    opt.MapFrom(src => src.DateOfBirth.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                });
            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                })
                .ForMember(dest => dest.PhoneNumber, opt => {
                    opt.MapFrom(d => d.PhoneNumber.FormatPhoneNumber());
                })
                .ForMember(dest => dest.ClassLevelId, opt => {
                    opt.MapFrom(src => src.Class.ClassLevelId);
                })
                .ForMember(dest => dest.ClassLevelName, opt => {
                    opt.MapFrom(src => src.ClassLevel.Name);
                })
                .ForMember(dest => dest.strDateOfBirth, opt => {
                    opt.MapFrom(d => d.DateOfBirth.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strGender, opt => {
                    opt.MapFrom(src => src.Gender == 0 ? "femme" : "homme");
                })
                .ForMember(dest => dest.ClassName, opt => {
                    opt.MapFrom(src => src.Class.Name);
                })
                .ForMember(dest => dest.DistrictName, opt => {
                    opt.MapFrom(src => src.District.Name);
                })
                .ForMember(dest => dest.CityName, opt => {
                    opt.MapFrom(src => src.City.Name);
                });
            CreateMap<User, UserForAccountDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                })
                .ForMember(dest => dest.PhoneNumber, opt => {
                    opt.MapFrom(d => d.PhoneNumber.FormatPhoneNumber());
                })
                .ForMember(dest => dest.SecondPhoneNumber, opt => {
                    opt.MapFrom(d => d.SecondPhoneNumber.FormatPhoneNumber());
                })
                .ForMember(dest => dest.UserTypeName, opt => {
                    opt.MapFrom(src => src.UserType.Name);
                });
            CreateMap<User, ChildForAccountDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                })
                .ForMember(dest => dest.strDateOfBirth, opt => {
                    opt.MapFrom(d => d.DateOfBirth.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.ClassName, opt => {
                    opt.MapFrom(src => src.Class.Name);
                })
                .ForMember(dest => dest.UserTypeName, opt => {
                    opt.MapFrom(src => src.UserType.Name);
                });
            CreateMap<Evaluation, EvalsForEditDto>()
                .ForMember(dest => dest.EvalTypeName, opt => {
                    opt.MapFrom(src => src.EvalType.Name);
                })
                .ForMember(dest => dest.EvalTypeAbbrev, opt => {
                    opt.MapFrom(src => src.EvalType.Abbrev);
                })
                .ForMember(dest => dest.EvalDateExpired, opt => {
                    opt.MapFrom(src => (src.EvalDate.Date <= DateTime.Now.Date));
                });
            CreateMap<Evaluation, EvalForEditDto>()
                .ForMember(dest => dest.EvalDate, opt => {
                    opt.MapFrom(src => src.EvalDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.EvalDateExpired, opt => {
                    opt.MapFrom(src => (src.EvalDate.Date <= DateTime.Now.Date));
                });
            CreateMap<UserEvaluation, ClassEvalGradesDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.User.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.StudentName, opt => {
                    opt.MapFrom(src => src.User.LastName + " " + src.User.FirstName);
                });
            CreateMap<UserEvaluation, GradeDto>()
                .ForMember(dest => dest.CourseName, opt => {
                    opt.MapFrom(src => src.Evaluation.Course.Name);
                })
                .ForMember(dest => dest.CourseAbbrev, opt => {
                    opt.MapFrom(src => src.Evaluation.Course.Abbreviation);
                })
                .ForMember(dest => dest.EvalDate, opt => {
                    opt.MapFrom(src => src.Evaluation.EvalDate.ToString("dd/MM/yy", frC));
                })
                .ForMember(dest => dest.EvalType, opt => {
                    opt.MapFrom(src => src.Evaluation.EvalType.Name);
                })
                .ForMember(dest => dest.EvalTypeAbbrev, opt => {
                    opt.MapFrom(src => src.Evaluation.EvalType.Abbrev);
                })
                .ForMember(dest => dest.EvalName, opt => {
                    opt.MapFrom(src => src.Evaluation.Name);
                })
                .ForMember(dest => dest.Grade, opt => {
                    opt.MapFrom(src => src.Grade);
                })
                .ForMember(dest => dest.GradeMax, opt => {
                    opt.MapFrom(src => src.Evaluation.MaxGrade);
                })
                .ForMember(dest => dest.Coeff, opt => {
                    opt.MapFrom(src => src.Evaluation.Coeff);
                });
            CreateMap<Class, ClassDetailDto>();
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<ClassLevel, ClassLevelDto>();
            CreateMap<User, TeacherInfoDto>()
                .ForMember(u => u.PhotoUrl, opt => opt
                  .MapFrom(u => u.Photos.FirstOrDefault (p => p.IsMain).Url));
            CreateMap<TeacherForEditDto, User>();
            CreateMap<User, TeacherForEditDto>()
                .ForMember(u => u.PhotoUrl, opt => opt
                    .MapFrom(u => u.Photos.FirstOrDefault (p => p.IsMain).Url))
                .ForMember(dest => dest.strDateOfBirth, opt => {
                    opt.MapFrom(d => d.DateOfBirth.ToString("dd/MM/yyyy", frC));
                });
            CreateMap<ImportUserDto, User>();
            CreateMap<QuickStudentAssignmentDto, User>();
            CreateMap<UserFromExelDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(m => m.SenderPhotoUrl, opt => opt
                  .MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(m => m.RecipientPhotoUrl, opt => opt
                  .MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
            CreateMap<Schedule, ClassDayCoursesDto>()
                .ForMember(dest => dest.TeacherName, opt => {
                  opt.MapFrom(src => src.Teacher.LastName + ' ' + src.Teacher.FirstName);
                })
                .ForMember(dest => dest.StartHourMin, opt => {
                  opt.MapFrom(d => d.StartHourMin.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.EndHourMin, opt => {
                  opt.MapFrom(d => d.EndHourMin.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.StartHourMinNum, opt => {
                  opt.MapFrom(d => Convert.ToInt32(d.StartHourMin.Hour.ToString() + d.StartHourMin.Minute.ToString()));
                })
                .ForMember(dest => dest.EndHourMinNum, opt => {
                  opt.MapFrom(d => Convert.ToInt32(d.EndHourMin.Hour.ToString() + d.EndHourMin.Minute.ToString()));
                })
                .ForMember(dest => dest.CourseName, opt => {
                  opt.MapFrom(d => d.Course.Name);
                })
                .ForMember(dest => dest.CourseAbbrev, opt => {
                  opt.MapFrom(d => d.Course.Abbreviation);
                })
                .ForMember(dest => dest.Color, opt => opt
                  .MapFrom(src => src.Course.Color));
            CreateMap<Schedule, ScheduleForTimeTableDto>()
                .ForMember(dest => dest.TeacherName, opt => {
                  opt.MapFrom(src => src.Teacher.LastName + ' ' + src.Teacher.FirstName);
                })
                .ForMember(dest => dest.StartHourMin, opt => {
                  opt.MapFrom(d => d.StartHourMin);
                })
                .ForMember(dest => dest.strStartHourMin, opt => {
                  opt.MapFrom(d => d.StartHourMin.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.strEndHourMin, opt => {
                  opt.MapFrom(d => d.EndHourMin.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.CourseName, opt => {
                  opt.MapFrom(d => d.Course.Name.Length > 10 ? d.Course.Abbreviation : d.Course.Name);
                })
                // .ForMember(dest => dest.Top, opt => {
                //   opt.MapFrom(d => d.StartHourMin.CalculateTop());
                // })
                .ForMember(dest => dest.Height, opt => {
                  opt.MapFrom(d => d.StartHourMin.CalculateHeight(d.EndHourMin));
                })
                .ForMember(dest => dest.Color, opt => opt
                  .MapFrom(src => src.Course.Color))
                .ForMember(s => s.ClassLevel, opt => opt
                  .MapFrom(d => d.Class.ClassLevel.Name))
                .ForMember(s => s.DelInfo, opt => opt
                  .MapFrom(d => d.Course.Name + " de " + d.StartHourMin.ToString("HH:mm", frC) +
                    " à " + d.EndHourMin.ToString("HH:mm", frC)));
            CreateMap<Schedule, ClassScheduleForTimeTableDto>()
                .ForMember(dest => dest.StartHourMin, opt => {
                  opt.MapFrom(d => d.StartHourMin.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.EndHourMin, opt => {
                  opt.MapFrom(d => d.EndHourMin.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.Top, opt => {
                  opt.MapFrom(d => d.StartHourMin.CalculateTop());
                })
                .ForMember(dest => dest.Height, opt => {
                  opt.MapFrom(d => d.StartHourMin.CalculateHeight(d.EndHourMin));
                })
                .ForMember(dest => dest.Color, opt => opt
                  .MapFrom(src => src.Course.Color))
                .ForMember(s => s.ClassLevel, opt => opt
                  .MapFrom(d => d.Class.ClassLevel.Name));
            CreateMap<Schedule, ScheduleToReturnDto>()
                .ForMember(dest => dest.CourseAbbrev, opt => {
                  opt.MapFrom(d => d.Course.Abbreviation);
                })
                .ForMember(dest => dest.strStartHourMin, opt => {
                  opt.MapFrom(d => d.StartHourMin.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.strEndHourMin, opt => {
                  opt.MapFrom(d => d.EndHourMin.ToString("HH:mm", frC));
                });
            CreateMap<Session, SessionToReturnDto>()
                .ForMember(dest => dest.CourseAbbrev, opt => {
                  opt.MapFrom(d => d.Course.Abbreviation);
                })
                .ForMember(dest => dest.strSessionDate, opt => {
                  opt.MapFrom(d => d.SessionDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.StartHourMin, opt => {
                  opt.MapFrom(d => d.StartHourMin.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.EndHourMin, opt => {
                  opt.MapFrom(d => d.EndHourMin.ToString("HH:mm", frC));
                });
            CreateMap<Email, EmailForListDto>()
                .ForMember(dest => dest.EmailType, opt => {
                  opt.MapFrom(d => d.EmailType.Name);
                })
                .ForMember(dest => dest.SentBy, opt => {
                  opt.MapFrom(d => d.InsertUser.LastName + ' ' + d.InsertUser.FirstName);
                })
                .ForMember(dest => dest.DateSent, opt => {
                  opt.MapFrom(d => d.InsertDate.ToString("dd/MM/yyyy", frC));
                });
            CreateMap<User, UserToValidateDto>()
                .ForMember(dest => dest.UserType, opt => {
                  opt.MapFrom(d => d.UserType.Name);
                });
            CreateMap<AgendaForSaveDto, Agenda>();
            CreateMap<SmsTemplateForSaveDto, SmsTemplate>(); 
            CreateMap<SmsTemplate, SmsTemplateForListDto>();
            CreateMap<EmailTemplateForSaveDto, EmailTemplate>(); 
            CreateMap<EmailTemplate, EmailTemplateForListDto>()
                .ForMember(dest => dest.EmailCategoryName, opt => {
                  opt.MapFrom(src => src.EmailCategory.Name);
                });
            CreateMap<AbsenceForSaveDto, Absence>();
            CreateMap<UserClassEvent, UserClassEventForListDto>()
                .ForMember(dest => dest.strStartDate, opt => {
                  opt.MapFrom(src => src.StartDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.StartTime, opt => {
                  opt.MapFrom(src => src.StartDate.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.strEndDate, opt => {
                  opt.MapFrom(src => src.EndDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.EndTime, opt => {
                  opt.MapFrom(src => src.EndDate.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.UserName, opt => opt
                  .MapFrom(src => src.User.LastName + ' ' + src.User.FirstName))
                .ForMember(dest => dest.DoneByName, opt => opt
                  .MapFrom(src => src.DoneBy.LastName + ' ' + src.DoneBy.FirstName))
                .ForMember(dest => dest.Justified, opt => {
                  opt.MapFrom(src => src.Justified == true ? "OUI" : "NON");
                });
            CreateMap<Absence, UserClassEventForListDto>()
                .ForMember(dest => dest.StartDate, opt => {
                    opt.MapFrom(src => src.StartDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.StartTime, opt => {
                    opt.MapFrom(src => src.StartDate.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.EndDate, opt => {
                    opt.MapFrom(src => src.EndDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.EndTime, opt => {
                    opt.MapFrom(src => src.EndDate.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.ClassEventType, opt => opt
                    .MapFrom(src => src.AbsenceType.Name))
                .ForMember(dest => dest.UserName, opt => opt
                    .MapFrom(src => src.User.LastName + ' ' + src.User.FirstName))
                .ForMember(dest => dest.DoneByName, opt => opt
                    .MapFrom(src => src.DoneBy.LastName + ' ' + src.DoneBy.FirstName))
                .ForMember(dest => dest.Justified, opt => {
                    opt.MapFrom(src => src.Justified == true ? "OUI" : "NON");
                });
            CreateMap<Absence, AbsencesToReturnDto>()
                .ForMember(dest => dest.StartDate, opt => {
                    opt.MapFrom(src => src.StartDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.StartTime, opt => {
                    opt.MapFrom(src => src.StartDate.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.EndDate, opt => {
                    opt.MapFrom(src => src.EndDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.EndTime, opt => {
                    opt.MapFrom(src => src.EndDate.ToString("HH:mm", frC));
                })
                .ForMember(dest => dest.AbsenceType, opt => opt
                    .MapFrom(src => src.AbsenceType.Name))
                .ForMember(dest => dest.UserName, opt => opt
                    .MapFrom(src => src.User.LastName + ' ' + src.User.FirstName))
                .ForMember(dest => dest.Justified, opt => {
                    opt.MapFrom(src => src.Justified == true ? "OUI" : "NON");
                });
            CreateMap<UserSanction, UserSanctionsToReturnDto>()
                .ForMember(dest => dest.SanctionDate, opt => {
                    opt.MapFrom(src => src.SanctionDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.SanctionName, opt => {
                    opt.MapFrom(src => src.Sanction.Name);
                })
                .ForMember(dest => dest.UserName, opt => opt
                    .MapFrom(src => src.User.LastName + ' ' + src.User.FirstName))
                .ForMember(dest => dest.SanctionedBy, opt => opt
                    .MapFrom(src => src.SanctionedBy.LastName + ' ' + src.SanctionedBy.FirstName));
            CreateMap<Evaluation, EvaluationForListDto>()
                .ForMember(dest => dest.CourseAbbrev, opt => {
                    opt.MapFrom(src => src.Course.Abbreviation);
                })
                .ForMember(dest => dest.CourseColor, opt => {
                    opt.MapFrom(src => src.Course.Color);
                })
                .ForMember(dest => dest.EvalDate, opt => {
                    opt.MapFrom(src => src.EvalDate.ToString("dd/MM/yyyy", frC));
                });
            CreateMap<ProductDto, Product>();
            CreateMap<DeadLineDto, DeadLine>();
            CreateMap<PayableDto, PayableAt>();
            CreateMap<CreateCoefficientDto, CourseCoefficient>();
            CreateMap<CourseCoefficient, CoefficientDetailsDto>()
                .ForMember(dest => dest.ClassLevelName, opt => {
                    opt.MapFrom(src => src.ClassLevel.Name);
                })
                .ForMember(dest => dest.CourseName, opt => {
                    opt.MapFrom(src => src.Course.Name);
                })
                .ForMember(dest => dest.ClassTypeName, opt => {
                    opt.MapFrom(src => src.ClassType.Name);
                });

                CreateMap<DeadLine, DealLineDetailsDto>()
                .ForMember(dest => dest.DueDate, opt => {
                    opt.MapFrom(src => src.DueDate.ToString("dd/MM/yyyy", frC));
                });
            CreateMap<ClassLevelProduct, ClasslevelProductDto>()
                .ForMember(dest => dest.ProductName, opt => {
                    opt.MapFrom(src => src.Product.Name);
                })
                .ForMember(dest => dest.LevelName, opt => {
                    opt.MapFrom(src => src.ClassLevel.Name);
                })
                .ForMember(dest => dest.DsplSeq, opt => {
                    opt.MapFrom(src => src.ClassLevel.DsplSeq);
                });
            CreateMap<Absence, AbsenceForCallSheetDto>()
                .ForMember(dest => dest.strStartDate, opt => {
                    opt.MapFrom(src => src.StartDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strEndDate, opt => {
                    opt.MapFrom(src => src.EndDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.LateInMin, opt => {
                    opt.MapFrom(src => (src.EndDate - src.StartDate).TotalMinutes);
                });
            CreateMap<Session, SessionForCallSheetDto>()
                .ForMember(dest => dest.strSessionDate, opt => {
                    opt.MapFrom(src => src.SessionDate.ToString("dd/MM/yyyy", frC));
                });
            CreateMap<FinOp, FinOpDto>()
                .ForMember(dest => dest.InvoiceNum, opt => {
                    opt.MapFrom(src => src.Invoice.InvoiceNum);
                })
                .ForMember(dest => dest.strFinOpDate, opt => {
                    opt.MapFrom(src => src.FinOpDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strInvoiceDate, opt => {
                    opt.MapFrom(src => src.Invoice.InvoiceDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.InvoiceAmount, opt => {
                    opt.MapFrom(src => src.Invoice.Amount);
                })
                .ForMember(dest => dest.ChequeNum, opt => {
                    opt.MapFrom(src => src.Cheque.ChequeNum);
                })
                .ForMember(dest => dest.ChequeBankName, opt => {
                    opt.MapFrom(src => src.Cheque.Bank.Name);
                })
                .ForMember(dest => dest.ChequeAmount, opt => {
                    opt.MapFrom(src => src.Cheque.Amount);
                })
                .ForMember(dest => dest.ChequePictureUrl, opt => {
                  opt.MapFrom(src => src.Cheque.PictureUrl);
                })
                .ForMember(dest => dest.FromBankName, opt => {
                  opt.MapFrom(src => src.FromBank.Name);
                })
                .ForMember(dest => dest.strAmount, opt => {
                  opt.MapFrom(src => src.Amount.ToString("N0") + " F");
                })
                .ForMember(dest => dest.PaymentTypeName, opt => {
                  opt.MapFrom(src => src.PaymentType.Name);
                })
                .ForMember(dest => dest.FromBankAccountName, opt => {
                  opt.MapFrom(src => src.FromBankAccount.Name);
                })
                .ForMember(dest => dest.FromCashDeskName, opt => {
                  opt.MapFrom(src => src.FromCashDesk.Name);
                })
                .ForMember(dest => dest.ToBankAccountName, opt => {
                  opt.MapFrom(src => src.ToBankAccount.Name);
                })
                .ForMember(dest => dest.ToCashDeskName, opt => {
                  opt.MapFrom(src => src.ToCashDesk.Name);
                });
            CreateMap<FinOpOrderLine, PaymentDto>()
                .ForMember(dest => dest.InvoiceNum, opt => {
                  opt.MapFrom(src => src.Invoice.InvoiceNum);
                })
                .ForMember(dest => dest.FinOpTypeId, opt => {
                  opt.MapFrom(src => src.FinOp.FinOpTypeId);
                })
                .ForMember(dest => dest.ProductId, opt => {
                  opt.MapFrom(src => src.OrderLine.Product.Id);
                })
                .ForMember(dest => dest.ProductName, opt => {
                  opt.MapFrom(src => src.OrderLine.Product.Name);
                })
                .ForMember(dest => dest.PaymentTypeId, opt => {
                  opt.MapFrom(src => src.FinOp.PaymentTypeId);
                })
                .ForMember(dest => dest.TypeName, opt => {
                  opt.MapFrom(src => src.FinOp.PaymentType.Name);
                })
                .ForMember(dest => dest.ChequeNum, opt => {
                  opt.MapFrom(src => src.FinOp.Cheque.ChequeNum);
                })
                .ForMember(dest => dest.ChequeBank, opt => {
                  opt.MapFrom(src => src.FinOp.Cheque.Bank.Name);
                })
                .ForMember(dest => dest.strFinOpDate, opt => {
                  opt.MapFrom(src => src.FinOp.FinOpDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.FromBankAccount, opt => {
                  opt.MapFrom(src => src.FinOp.FromBankAccount.Name);
                })
                .ForMember(dest => dest.FromCashDesk, opt => {
                  opt.MapFrom(src => src.FinOp.FromCashDesk.Name);
                })
                .ForMember(dest => dest.ToBankAccount, opt => {
                  opt.MapFrom(src => src.FinOp.ToBankAccount.Name);
                })
                .ForMember(dest => dest.ToCashDesk, opt => {
                  opt.MapFrom(src => src.FinOp.ToCashDesk.Name);
                })
                .ForMember(dest => dest.ChildLastName, opt => {
                  opt.MapFrom(src => src.OrderLine.Child.LastName.FirstLetterToUpper());
                })
                .ForMember(dest => dest.ChildFirstName, opt => {
                  opt.MapFrom(src => src.OrderLine.Child.FirstName.FirstLetterToUpper());
                })
                .ForMember(dest => dest.Cashed, opt => {
                  opt.MapFrom(src => src.FinOp.Cashed);
                })
                .ForMember(dest => dest.Received, opt => {
                  opt.MapFrom(src => src.FinOp.Received);
                })
                .ForMember(dest => dest.DepositedToBank, opt => {
                  opt.MapFrom(src => src.FinOp.DepositedToBank);
                })
                .ForMember(dest => dest.Rejected, opt => {
                  opt.MapFrom(src => src.FinOp.Rejected);
                });
            CreateMap<OrderLine, LineUserToValidateDto>()
                .ForMember(dest => dest.LastName, opt => {
                  opt.MapFrom(src => src.Child.LastName.FirstLetterToUpper());
                })
                .ForMember(dest => dest.FirstName, opt => {
                  opt.MapFrom(src => src.Child.FirstName.FirstLetterToUpper());
                })
                .ForMember(dest => dest.ClassLevelName, opt => {
                  opt.MapFrom(src => src.ClassLevel.Name);
                })
                .ForMember(dest => dest.ClassName, opt => {
                  opt.MapFrom(src => src.Child.Class.Name);
                })
                .ForMember(dest => dest.Age, opt => {
                  opt.MapFrom(src => src.Child.DateOfBirth.CalculateAge());
                })
                .ForMember(dest => dest.PhotoUrl, opt => {
                  opt.MapFrom(src => src.Child.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.NbDaysLate, opt => {
                  opt.MapFrom(src => (src.Validity - DateTime.Now).TotalDays + 1);
                })
                .ForMember(dest => dest.strAmountTTC, opt => {
                  opt.MapFrom(src => src.AmountTTC.ToString("N0") + " F");
                });
            CreateMap<OrderLine, OrderLineDto>()
                .ForMember(dest => dest.ProductName, opt => {
                  opt.MapFrom(src => src.Product.Name);
                })
                .ForMember(dest => dest.ChildLastName, opt => {
                  opt.MapFrom(src => src.Child.LastName.FirstLetterToUpper());
                })
                .ForMember(dest => dest.ChildFirstName, opt => {
                  opt.MapFrom(src => src.Child.FirstName.FirstLetterToUpper());
                })
                .ForMember(dest => dest.ClassLevelName, opt => {
                  opt.MapFrom(src => src.ClassLevel.Name);
                })
                .ForMember(dest => dest.ChildClassName, opt => {
                  opt.MapFrom(src => src.Child.Class.Name);
                })
                .ForMember(dest => dest.ChildAge, opt => {
                  opt.MapFrom(src => src.Child.DateOfBirth.CalculateAge());
                })
                .ForMember(dest => dest.ChildPhotoUrl, opt => {
                  opt.MapFrom(src => src.Child.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.NbDaysLate, opt => {
                  opt.MapFrom(src => (src.Validity - DateTime.Now).TotalDays + 1);
                })
                .ForMember(dest => dest.strAmountHT, opt => {
                  opt.MapFrom(src => src.AmountHT.ToString("N0") + " F");
                })
                .ForMember(dest => dest.strAmountTTC, opt => {
                  opt.MapFrom(src => src.AmountTTC.ToString("N0") + " F");
                });
            CreateMap<Order, OrderUserToValidateDto>()
                .ForMember(dest => dest.strOrderDate, opt => {
                  opt.MapFrom(src => src.OrderDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strDeadline, opt => {
                  opt.MapFrom(src => src.Deadline.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strDeadline, opt => {
                  opt.MapFrom(src => src.Deadline.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.NbDaysLate, opt => {
                  opt.MapFrom(src => (src.Validity - DateTime.Now).TotalDays + 1);
                })
                .ForMember(dest => dest.NbChildren, opt => {
                  opt.MapFrom(src => src.Lines.Count());
                })
                .ForMember(dest => dest.strValidity, opt => {
                  opt.MapFrom(src => src.Validity.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strAmountTTC, opt => {
                  opt.MapFrom(src => src.AmountTTC.ToString("N0") + " F");
                });
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.strOrderDate, opt => {
                  opt.MapFrom(src => src.OrderDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strDeadline, opt => {
                  opt.MapFrom(src => src.Deadline.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strDeadline, opt => {
                  opt.MapFrom(src => src.Deadline.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.NbDaysLate, opt => {
                  opt.MapFrom(src => (src.Validity - DateTime.Now).TotalDays + 1);
                })
                .ForMember(dest => dest.strValidity, opt => {
                  opt.MapFrom(src => src.Validity.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.ChildLastName, opt => {
                  opt.MapFrom(src => src.Child.LastName);
                })
                .ForMember(dest => dest.ChildFirstName, opt => {
                  opt.MapFrom(src => src.Child.FirstName);
                })
                .ForMember(dest => dest.ChildClassName, opt => {
                  opt.MapFrom(src => (src.Child.Class.Name));
                })
                .ForMember(dest => dest.FatherLastName, opt => {
                  opt.MapFrom(src => (src.Father.LastName));
                })
                .ForMember(dest => dest.FatherFirstName, opt => {
                  opt.MapFrom(src => (src.Father.FirstName));
                })
                .ForMember(dest => dest.FatherCell, opt => {
                  opt.MapFrom(src => (src.Father.PhoneNumber));
                })
                .ForMember(dest => dest.FatherEmail, opt => {
                  opt.MapFrom(src => (src.Father.Email));
                })
                .ForMember(dest => dest.MotherLastName, opt => {
                  opt.MapFrom(src => (src.Mother.LastName));
                })
                .ForMember(dest => dest.MotherFirstName, opt => {
                  opt.MapFrom(src => (src.Mother.FirstName));
                })
                .ForMember(dest => dest.MotherCell, opt => {
                  opt.MapFrom(src => (src.Mother.PhoneNumber));
                })
                .ForMember(dest => dest.MotherEmail, opt => {
                  opt.MapFrom(src => src.Mother.Email);
                })
                .ForMember(dest => dest.strAmountHT, opt => {
                  opt.MapFrom(src => src.AmountHT.ToString("N0") + " F");
                })
                .ForMember(dest => dest.strAmountTTC, opt => {
                  opt.MapFrom(src => src.AmountTTC.ToString("N0") + " F");
                });
            CreateMap<OrderLineDeadline, OrderLineDeadlineDto>()
                .ForMember(dest => dest.strDueDate, opt => {
                  opt.MapFrom(src => src.DueDate.ToString("dd/MM/yyyy", frC));
                });
            CreateMap<User, ParentUserDto>()
                .ForMember(dest => dest.Cell, opt => {
                  opt.MapFrom(src => src.PhoneNumber);
                });
            CreateMap<User, ChildParentDto>()
                .ForMember(dest => dest.ClassName, opt => {
                  opt.MapFrom(src => src.Class.Name);
                })
                .ForMember(dest => dest.ClassLevelName, opt => {
                  opt.MapFrom(src => src.ClassLevel.Name);
                })
                .ForMember(dest => dest.Cell, opt => {
                  opt.MapFrom(src => src.PhoneNumber);
                });
        }
    }
}