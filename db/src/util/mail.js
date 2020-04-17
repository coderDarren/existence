'use strict'
const nodemailer = require('nodemailer');
const {google} = require('googleapis')
const OAuth2 = google.auth.OAuth2;
const mailUser = process.env.MAIL_USER;
const mailClient = process.env.MAIL_CLIENT;
const mailSecret = process.env.MAIL_SECRET;
const mailRefresh = process.env.MAIL_REFRESH;

const sendVerificationEmail = async function(_to, _verificationID, _username, _deviceModel, _deviceName) {
    const _oauth2Client = new OAuth2(mailClient, mailSecret, "https://developers.google.com/oauthplayground");
    _oauth2Client.setCredentials({
        refresh_token: mailRefresh
    });

    const _accessToken = _oauth2Client.getAccessToken();
    const _actionLink = `https://verify.notfungames.com?ID=${_verificationID}`;
    
    const transporter = nodemailer.createTransport({
        service: "gmail",
        auth: {
            type: 'OAuth2',
            user: mailUser,
            clientId: mailClient,
            clientSecret: mailSecret,
            refreshToken: mailRefresh,
            _accessToken: _accessToken
        }
    });

    // send mail with defined transport object
    const _success = await transporter.sendMail({
        from: mailUser,
        to: _to,
        subject: `notfun games - New Device Login (${_deviceModel})`,
        html: `
        <div style='max-width:500px;'>
            <div style='background-color:#444;'>
                <img src="https://artifacts.notfungames.com/logo256_light.png" 
                    align="middle"
                    style='display:block;margin:0 auto;width:100px;height:100px;'>
            </div>
            <h3 style='text-align:center;'>Sorry to bother you, ${_username}</h3>
            <h3 style='text-align:center;'>Was This You?</h3>
            <p style='text-align:center;'>${_deviceModel} - ${_deviceName}</p>
            <p style='text-align:center;'>An attempt to access your account was made by this untrusted device. 
            Ignore this email if you did not try to log in from a new device. 
            To verify the new device, click the link below.</p>
            <div style='position:relative;left:50%;transform:translateX(-50%);margin-bottom:24px;align-items:middle;'>
                <a href="https://publicassets.notfungames.com/en-tapshift-privacypolicy.pdf" 
                style='text-align:center;font-size:12px;color:#aaa;'>Privacy Policy</a>
            </div>
            <a style='text-decoration:none;' href=${_actionLink}>
                <div style='
                background-color:#4087ff;
                border-radius:12px;
                padding:14px;
                color:#fff;
                font-size:18px;
                font-weight:bold;
                cursor:pointer;
                position:relative;
                text-align:center;
                '>Verify</div>
            <a/>
            <div style='margin-top:50px'>
                <p style='text-align:center;font-size:12px;color:#aaa;'>RENAISSANCE CODERS, LLC</p>
                <p style='text-align:center;font-size:12px;color:#aaa;'>ALL RIGHTS RESERVED.</p>
            </div>
        </div>
        ` // html body
    }).then(
        function(_resp) {
            console.log(_resp);
            return true;
        }
    ).catch(
        function(_err) {
            //console.log(_err);
            return false;
        }
    )

    transporter.close();

    console.log('Message sent: '+_success);
    return _success;
}

//sendVerificationEmail('doneale3@gmail.com', 'abc-123-def-456', 'doneale54', 'model', 'name');

module.exports = {
    sendVerificationEmail
}