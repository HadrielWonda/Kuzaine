﻿using Helpers;
using Services;



namespace Kuzaine.Builders.Bff.Components.Notifications;

public class NotificationsComponentBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public NotificationsComponentBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateNotificationComponentItems(string spaDirectory)
    {
        var indexCassPath = ClassPathHelper.BffSpaComponentClassPath(spaDirectory, "Notifications", "index.ts");
        var indexFileText = GetNotificationIndexText();
        _utilities.CreateFile(indexCassPath, indexFileText);

        var privateNotificationClassPath = ClassPathHelper.BffSpaComponentClassPath(spaDirectory, "Notifications", "Notifications.tsx");
        var privateNotificationFileText = GetNotificationText();
        _utilities.CreateFile(privateNotificationClassPath, privateNotificationFileText);
    }

    public static string GetNotificationIndexText()
    {
        return @$"export * from './Notifications';";
    }

    public static string GetNotificationText()
    {
        return @$"import React from 'react';
import {{ toast, ToastContainer, ToastOptions }} from 'react-toastify';

const Notifications = () => {{
	return (
		<ToastContainer position={{toast.POSITION.TOP_RIGHT}} hideProgressBar={{true}} theme='colored' />
	);
}};

Notifications.success = (message: string, options?: ToastOptions<{{}}>) => {{
	toast.success(
		<div className='mx-2'>{{message}}</div>,
		Object.assign(
			{{
				bodyClassName: 'py-3',
			}},
			options
		)
	);
}};

Notifications.error = (message: string, options?: ToastOptions<{{}}>) => {{
	toast.error(
		<div className='mx-2'>{{message}}</div>,
		Object.assign(
			{{
				bodyClassName: 'py-3',
			}},
			options
		)
	);
}};

export {{ Notifications }};
";
    }
}
