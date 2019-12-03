"""
The flask application package.
"""
from flask_restful import reqparse, abort, Api, Resource
import nltk
import json
from nltk.corpus import stopwords
import imaplib
import email
import re,string
from flask import Flask,request

app = Flask(__name__)
api = Api(app)


class PosTag(Resource):
    def get(self):

        raw_query = request.args['query']
        stop_words = set(stopwords.words('english'))
        tokenized_words = nltk.word_tokenize(raw_query)
        filtered_query = [w for w in tokenized_words if not w in stop_words]
        
        try:
            #for item in filtered_query:
            tagged = nltk.pos_tag(filtered_query)
        except Exception as e:
            return(str(e))
        return tagged

class EmailService(Resource):
    def get(self):
        mail=imaplib.IMAP4_SSL('imap.gmail.com')
        mail.login('varunreddycs@gmail.com','7799142118')
        mail.list()
        mail.select('inbox')
        result,data=mail.uid('search',None,"ALL")
       
        i=len(data[0].split())
        for x in range(i):
            latest_email_uid=data[0].split()[x]
            result,email_data=mail.uid('fetch',latest_email_uid,'(RFC822)')
            raw_email=email_data[0][1]
            raw_email_string=raw_email.decode('utf-8')
            email_message=email.message_from_string(raw_email_string)
            b=email_message.get_from()
            for part in email_message.walk():
                if part.get_content_type() == "text/plain": 
                    body=part.get_payload(decode=True)
                    save_string=str("E:Dumpgmailemail_" + str(x) + ".eml")
                    myfile = open(save_string, 'a')
                    myfile.write(body.decode('cp1252'))
                    myfile.close()
                else:
                    continue
        return "DONE"  

class ParseMail(Resource):
    def get(self):
        mail=imaplib.IMAP4_SSL('imap.gmail.com')
        mail.login('varun.pishati@imaginea.com','varun1995@REDDY')
        mail.list()
        mail.select('inbox')
        result,data=mail.search(None,'(From "Pavan Bharadwaj")')
        ids=data[0]
        id_list=ids.split()
        latest_email_id=id_list[-1]
        result, data = mail.fetch(latest_email_id, "(RFC822)")
        raw_email = data[0][1]
        raw_email_string=raw_email.decode('utf-8')
        x=""

        email_message=email.message_from_string(raw_email_string)
        for part in email_message.walk():
            body=str("" if part.get_payload(decode=True) is None else part.get_payload(decode=True))
            
            if body.startswith('<html') or body.startswith("b'<html"):
                x=body
        pattern=r":9.0pt[\\]{0,3}[']>+([A-Z a-z 0-9 -]{0,32})"
        regex=re.compile(pattern,re.IGNORECASE | re.DOTALL)
        x=x[2:]
        m=regex.search(x)
        #v=regex.match(x)
        #xv=re.search(r'\bfont-size:9.0pt\'>+([A-Z a-z 0-9 -]{0,32})',x)
        #xd=re.match(r'\bfont-size:9.0pt\'>+([A-Z a-z 0-9 -]{0,32})',x)
        #match=re.search(r'\bfont-size:9.0pt\'>+([A-Z a-z 0-9 -]{0,32})',x)
        #c=regex.finditer(x)
        i=0
        contents=regex.findall(x)    
        contentNames=['EmployeeID','EmployeeName','ProjectName','GroupName','StartDate','BillingState']
        
        for match in regex.finditer(x):
            obj=match.start()
            obj=match.group(1)
        #for item in contents:
        #    contentNameitem=contentNames[i]
        #    value=contentNameitem+":"+item
        #    i=i+1
        #    dic['{}'].append(value)
        mydic2={contentNames[0]:contents[0],contentNames[1]:contents[1],contentNames[2]:contents[2],contentNames[3]:contents[3],contentNames[4]:contents[4],contentNames[5]:contents[5]}
        return mydic2


                




api.add_resource(PosTag, '/postag')
api.add_resource(EmailService, '/emailContent')
api.add_resource(ParseMail, '/parsemail')




import PosTagService.views
