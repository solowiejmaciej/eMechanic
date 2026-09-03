namespace eMechanic.NotificationService.Helpers;

public static class EmailTemplateBuilder
{
    public static string Build(string title, string content)
    {


        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
        </head>
        <body style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f6; margin: 0; padding: 20px 0;'>
            <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f7f6; margin: 0; padding: 0;'>
                <tr>
                    <td align='center'>
                        <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.05); margin: 20px auto; text-align: left;'>
                            <!-- Header -->
                            <tr>
                                <td style='background-color: #2c3e50; padding: 25px 30px; text-align: center;'>
                                    <h1 style='color: #ffffff; margin: 0; font-size: 26px; letter-spacing: 1px;'>eMechanic</h1>
                                </td>
                            </tr>

                            <!-- Body -->
                            <tr>
                                <td style='padding: 40px 30px; color: #374151; line-height: 1.6; font-size: 16px;'>
                                    <h2 style='color: #2980b9; margin-top: 0; font-size: 22px;'>{title}</h2>
                                    {content}

                                </td>
                            </tr>

                            <!-- Footer -->
                            <tr>
                                <td style='background-color: #f9fafb; padding: 20px 30px; text-align: center; font-size: 13px; color: #6b7280; border-top: 1px solid #e5e7eb;'>
                                    <p style='margin: 0 0 5px 0;'>Wiadomość wygenerowana automatycznie przez system <strong>eMechanic</strong>.</p>
                                    <p style='margin: 0;'>Prosimy na nią nie odpowiadać.</p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>";
    }
}
