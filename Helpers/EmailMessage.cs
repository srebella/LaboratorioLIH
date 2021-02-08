
using System;
using System.Collections.Generic;

public sealed class EmailMessage  
{  
    private readonly List<string> to;  
    private readonly List<string> cc;  
    private readonly List<string> bcc;  
    private readonly List<string> attachments;  
  
    public EmailMessage(string subject, string from, string body, List<string> to,  
        List<string> cc, List<string> bcc, List<string> attachments)  
    {  
        if (string.IsNullOrEmpty(subject))  
            throw new ArgumentException("Subject must be set");  
  
        if (string.IsNullOrEmpty(from))  
            throw new ArgumentException("From must be set");  
  
        if (string.IsNullOrEmpty(body))  
            throw new ArgumentException("Body must be set");  
  
        if (to.Count == 0)  
            throw new ArgumentException("At least one To must be set");  
  
        this.Subject = subject;  
        this.From = from;  
        this.Body = body;  
        this.to = to;  
        this.cc = cc;  
        this.bcc = bcc;  
        this.attachments = attachments;  
    }  
  
    public string Subject { get; }  
    public string From { get; }  
    public string Body { get; }  
    public System.Collections.Generic.IReadOnlyList<string> To => to;  
    public System.Collections.Generic.IReadOnlyList<string> CC => cc;  
    public System.Collections.Generic.IReadOnlyList<string> BCC => bcc;  
    public System.Collections.Generic.IReadOnlyList<string> Attachments => attachments;  
}  