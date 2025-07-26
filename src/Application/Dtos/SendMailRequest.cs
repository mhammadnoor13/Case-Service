using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos;

public record SendMailRequest(string Recipient, string Subject, string Body);
