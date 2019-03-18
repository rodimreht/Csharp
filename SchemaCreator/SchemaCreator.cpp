/*
**  SchemaCreator.cpp: Installs an Active Directory Schema extension.
**
**  Written by: thermidor (thermidor@nets.co.kr)
**
**  Created on: 2003-03-27
**
**  Last Updated : 2004-03-25
**
**  History :
**
**  -- 2003-03-31 : Add Single / Multi value
**  -- 2003-03-31 : Add Parent Class & fix failure bug for creating classes only
**  -- 2003-08-18 : Add ADAM version & localhost only
**  -- 2003-10-23 : Add User-Defined TCP Port number
**  -- 2003-11-05 : Fix a bug
**  -- 2004-03-25 : Fix port number bug
**
**  Notes:        Must be run on a DC which is the current Schema Master and has
**                schema updates enabled using the registry key and value:
**
**                KEY:  HKLM\CurrentControlSet\Services\NTDS\Parameters
**                Value:    Schema Update Allowed, REG_DWORD, 1
**
**  Libraries:  Activeds.lib, Adsiid.lib
**
*/

#include "stdafx.h"
#include <windows.h>
#include <ole2.h>
#include <iads.h>
#include <activeds.h>
#include <stdio.h>
#include <atlbase.h>
#include <fcntl.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <io.h>
#include <wchar.h>
#include "SchemaCreator.h"


// Data structures for creating schema objects
//
// attribute values: these are unions and cannot be statically initialized.
//
ADSVALUE		cn,
				singleValued,
				oid,
				syntax,
				omSyntax,
				ldapname,
				idGuid,
				objectClass,
				objectClassCategory,
				subClassOf,
				defaultSecurityDesc,
				defaultHidingValue;

/****************************
 * bufArray for attributes
 * 
 * 0 : id (ATR)
 * 1 : multi-value (M or S)
 * 2 : attribute name
 * 3 : ldap display name
 * 4 : attribute oid
 * 5 : type
 * 6 : guid
 *-------------------------- 
 * bufArray for classes
 * 
 * 0 : id (CLS)
 * 1 : class name
 * 2 : class oid
 * 3 : parent class name
 ****************************/
WCHAR bufArray[200][7][256];	// maximum attribute or class number : 200
ULONG tcpPort = 389;

// Create an attribute
HRESULT CreateAttribute(LPCWSTR pwszMulti, LPCWSTR pwszName, LPCWSTR pwszDisplayName, 
						LPCWSTR pwszOID, LPCWSTR pwszType, 
						ADS_ATTR_INFO *pattrArray, DWORD dwAttrs,
						/*GUID attrGUID,*/ IDispatch *pDisp, IDirectoryObject *pSchema)
{
	HRESULT hr;
	LPWSTR pwszTemp = NULL;

	cn.dwType = ADSTYPE_CASE_IGNORE_STRING;
	cn.CaseIgnoreString = (ADS_CASE_IGNORE_STRING) pwszName;

	if (!lstrcmpW(pwszMulti, L"M"))	// Multi-value enabled
	{
		singleValued.dwType = ADSTYPE_BOOLEAN;
		singleValued.Boolean = VARIANT_FALSE;
	}
	else
	{
		singleValued.dwType = ADSTYPE_BOOLEAN;
		singleValued.Boolean = VARIANT_TRUE;
	}

	oid.dwType = ADSTYPE_CASE_IGNORE_STRING;
	oid.CaseIgnoreString = (ADS_CASE_IGNORE_STRING) pwszOID; // Reserved test OID
	objectClass.dwType = ADSTYPE_CASE_IGNORE_STRING;
	objectClass.CaseIgnoreString = L"attributeSchema";

	if (!lstrcmpW(pwszType, L"BOOLEAN"))
	{
		syntax.dwType = ADSTYPE_CASE_IGNORE_STRING;
		syntax.CaseIgnoreString = L"2.5.5.8";				// 2.5.5.8 = Boolean
		omSyntax.dwType = ADSTYPE_INTEGER;
		omSyntax.Integer = 1;
	}
	else if (!lstrcmpW(pwszType, L"INTEGER"))
	{
		syntax.dwType = ADSTYPE_CASE_IGNORE_STRING;
		syntax.CaseIgnoreString = L"2.5.5.9";				// 2.5.5.9 = Integer
		omSyntax.dwType = ADSTYPE_INTEGER;
		omSyntax.Integer = 2;
	}
	else if (!lstrcmpW(pwszType, L"OCTET"))
	{
		syntax.dwType = ADSTYPE_CASE_IGNORE_STRING;
		syntax.CaseIgnoreString = L"2.5.5.10";				// 2.5.5.10 = OctetString
		omSyntax.dwType = ADSTYPE_INTEGER;
		omSyntax.Integer = 4;
	}
	else if (!lstrcmpW(pwszType, L"STRING"))
	{
		syntax.dwType = ADSTYPE_CASE_IGNORE_STRING;
		syntax.CaseIgnoreString = L"2.5.5.12";				// 2.5.5.12 = DirectoryString
		omSyntax.dwType = ADSTYPE_INTEGER;
		omSyntax.Integer = 64;
	}
	else
	{
		return S_FALSE;
	}

	//
	// The LDAP display name is defaulted by the server and should not be
	// provided unless different than the name computed from the CN
	// attribute - the LDAP name is the CN with the hyphens removed and 
	// case delimiting substituted. The initial character is always lowercase.
	// For this example, an explicit LDAP display name is provided to illustrate
	// how it is done.
	ldapname.dwType = ADSTYPE_CASE_IGNORE_STRING;
	ldapname.CaseIgnoreString = (ADS_CASE_IGNORE_STRING) pwszDisplayName;

	//
	// Schema-ID-Guid is provided by the server is the client does not
	// provide it. This is a good example of how to write an Octet String to the DS.
	//idGuid.dwType = ADSTYPE_OCTET_STRING;
	//idGuid.OctetString.dwLength = sizeof(attrGUID);
	//idGuid.OctetString.lpValue = (LPBYTE)&attrGUID;

	pwszTemp = new WCHAR[32 + lstrlenW(pwszName) + 1];
	if(!pwszTemp)
	{
		return S_FALSE;
	}
	wmemset( pwszTemp, 0x00, 32 + lstrlenW(pwszName) + 1 );
	lstrcpyW(pwszTemp, L"cn=");
	lstrcatW(pwszTemp, pwszName);

	hr = pSchema->CreateDSObject(pwszTemp,
									pattrArray,
									dwAttrs,
									&pDisp);
	if (FAILED(hr))
	{
		if (pwszTemp)
		{
			pwszTemp = NULL;
			delete pwszTemp;
		}

		pwszTemp = new WCHAR[lstrlenW(pwszName) + 64];
		if (!pwszTemp)
		{
			return S_FALSE;
		}
		wmemset( pwszTemp, 0x00, lstrlenW(pwszName) + 64 );
		lstrcpyW(pwszTemp, L"Create Attribute ");
		lstrcatW(pwszTemp, pwszName);
		lstrcatW(pwszTemp, L" failed.");

		ReportErrorW(pwszTemp, hr);

		if (pwszTemp)
		{
			pwszTemp = NULL;
			delete pwszTemp;
		}

		return hr;
	} 
	else 
	{
		if (pwszTemp)
		{
			pwszTemp = NULL;
			delete pwszTemp;
		}

		pwszTemp = new WCHAR[lstrlenW(pwszName) + 64];
		if (!pwszTemp)
		{
			return S_FALSE;
		}
		wmemset( pwszTemp, 0x00, lstrlenW(pwszName) + 64 );
		lstrcpyW(pwszTemp, L"\n");
		lstrcatW(pwszTemp, pwszName);
		lstrcatW(pwszTemp, L" Attribute defined.\n");

		wprintf(pwszTemp);

		// The IDispatch interface returned by the CreateDSObject call is not used.
		// Release it now.
		if (pDisp)
		{
			pDisp->Release();
		}
		pDisp = NULL;

		if (pwszTemp)
		{
			pwszTemp = NULL;
			delete pwszTemp;
		}

		return S_OK;
	}
}


// Create a class
HRESULT CreateClass(LPCWSTR pwszName, LPCWSTR pwszOID, LPCWSTR pwszParent,
                    ADS_ATTR_INFO *pclassArray, DWORD dwAttrs,
					IDispatch *pDisp, IDirectoryObject *pSchema)
{
	HRESULT hr;
	LPWSTR pwszTemp = NULL;

	cn.dwType = ADSTYPE_CASE_IGNORE_STRING;
	cn.CaseIgnoreString = (ADS_CASE_IGNORE_STRING) pwszName;
	objectClass.dwType = ADSTYPE_CASE_IGNORE_STRING;
	objectClass.CaseIgnoreString = L"classSchema";
	oid.dwType = ADSTYPE_CASE_IGNORE_STRING;
	oid.CaseIgnoreString = (ADS_CASE_IGNORE_STRING) pwszOID; // Reserved Test OID.

	if (pwszParent[0] == '\0')	// default parent class : top
	{
		subClassOf.dwType = ADSTYPE_CASE_IGNORE_STRING;
		subClassOf.CaseIgnoreString = L"top";
	}
	else
	{
		subClassOf.dwType = ADSTYPE_CASE_IGNORE_STRING;
		subClassOf.CaseIgnoreString = (ADS_CASE_IGNORE_STRING) pwszParent;
	}

	defaultSecurityDesc.dwType = ADSTYPE_CASE_IGNORE_STRING;
	defaultSecurityDesc.CaseIgnoreString = L"D:(A;;RPWPCRCCDCLCLORCWOWDSDDTSW;;;DA)(A;;RPWPCRCCDCLCLORCWOWDSDDTSW;;;SY)(A;;RPLCLORC;;;AU)S:(AU;SAFA;WDWOSDDTWPCRCCDCSW;;;WD)";
	defaultHidingValue.dwType = ADSTYPE_BOOLEAN;
	defaultHidingValue.Boolean = -1;

	// Object-Class-Category=
	// 88_CLASS           0
	// STRUCTURAL_CLASS   1
	// ABSTRACT_CLASS     2
	// AUXILIARY_CLASS    3
	objectClassCategory.dwType = ADSTYPE_INTEGER;
	objectClassCategory.Integer = 1; 

	pwszTemp = new WCHAR[32 + lstrlenW(pwszName) + 1];
	if (!pwszTemp)
	{
		return S_FALSE;
	}
	wmemset( pwszTemp, 0x00, 32 + lstrlenW(pwszName) + 1 );
	lstrcpyW(pwszTemp, L"cn=");
	lstrcatW(pwszTemp, pwszName);

	hr = pSchema->CreateDSObject(pwszTemp,
									pclassArray,
									dwAttrs,
									&pDisp);
	if (FAILED(hr))
	{
		if (pwszTemp)
		{
			pwszTemp = NULL;
			delete pwszTemp;
		}

		pwszTemp = new WCHAR[lstrlenW(pwszName) + 64];
		if (!pwszTemp)
		{
			return S_FALSE;
		}
		wmemset( pwszTemp, 0x00, lstrlenW(pwszName) + 64 );
		lstrcpyW(pwszTemp, L"Create Class ");
		lstrcatW(pwszTemp, pwszName);
		lstrcatW(pwszTemp, L" failed.");

		ReportErrorW(pwszTemp, hr);

		if (pwszTemp)
		{
			pwszTemp = NULL;
			delete pwszTemp;
		}

		return hr;
	} 
	else 
	{
		if (pwszTemp)
		{
			pwszTemp = NULL;
			delete pwszTemp;
		}

		pwszTemp = new WCHAR[lstrlenW(pwszName) + 64];
		if (!pwszTemp)
		{
			return S_FALSE;
		}
		wmemset( pwszTemp, 0x00, lstrlenW(pwszName) + 64 );
		lstrcpyW(pwszTemp, L"\n");
		lstrcatW(pwszTemp, pwszName);
		lstrcatW(pwszTemp, L" Class defined.\n");

		wprintf(pwszTemp);

		// The IDispatch interface returned by the CreateDSObject call is not used.
		// Release it now.
		if (pDisp)
		{
			pDisp->Release();
		}
		pDisp = NULL;

		if (pwszTemp)
		{
			pwszTemp = NULL;
			delete pwszTemp;
		}

		return S_OK;
	}
}


// Update class for adding attributes
HRESULT UpdateClass(LPCWSTR pwszName, LPCWSTR pwszRoot, 
					ADS_ATTR_INFO *pattrArray)
{
	HRESULT hr;
	LPWSTR pwszTemp = NULL;
	IDirectoryObject *pSchemaUpdate;
	ULONG iAttrsMod;

	pwszTemp = new WCHAR[32 + lstrlenW(pwszName) + lstrlenW(pwszRoot) + 1];
	if(!pwszTemp)
	{
		return S_FALSE;
	}
	wmemset( pwszTemp, 0x00, 32 + lstrlenW(pwszName) + lstrlenW(pwszRoot) + 1 );

	if (tcpPort != 389)
		swprintf(pwszTemp, L"LDAP://localhost:%lu/CN=", tcpPort);
	else
		lstrcpyW(pwszTemp, L"LDAP://localhost/CN=");

	lstrcatW(pwszTemp, pwszName);
	lstrcatW(pwszTemp, L",");
	lstrcatW(pwszTemp, pwszRoot);

	// Bind to the schema container and get the IDirectoryObject interface
	// on it.
	hr = ADsGetObject(pwszTemp,
						IID_IDirectoryObject,
						(void**)&pSchemaUpdate);
	if (FAILED(hr))
	{
		wprintf(L"LDAP binding (for Update) failed!\n");
		return S_FALSE;
	}

	hr = pSchemaUpdate->SetObjectAttributes(pattrArray,	1, &iAttrsMod);

	if (FAILED(hr))
	{
		if (pwszTemp)
		{
			pwszTemp = NULL;
			delete pwszTemp;
		}

		pwszTemp = new WCHAR[lstrlenW(pwszName) + 64];
		if (!pwszTemp)
		{
			return S_FALSE;
		}
		wmemset( pwszTemp, 0x00, lstrlenW(pwszName) + 64 );
		lstrcpyW(pwszTemp, L"Update Class ");
		lstrcatW(pwszTemp, pwszName);
		lstrcatW(pwszTemp, L" object failed.");

		ReportErrorW(pwszTemp, hr);

		if(pSchemaUpdate)
		{
			pSchemaUpdate->Release();
		}

		if (pwszTemp)
		{
			pwszTemp = NULL;
			delete pwszTemp;
		}

		return hr;
	} 
	else 
	{
		if (pwszTemp)
		{
			pwszTemp = NULL;
			delete pwszTemp;
		}

		pwszTemp = new WCHAR[lstrlenW(pwszName) + 64];
		if (!pwszTemp)
		{
			return S_FALSE;
		}
		wmemset( pwszTemp, 0x00, lstrlenW(pwszName) + 64 );
		lstrcpyW(pwszTemp, L"\nUpdated ");
		lstrcatW(pwszTemp, pwszName);
		lstrcatW(pwszTemp, L" Class object.\n");

		wprintf(pwszTemp);

		if(pSchemaUpdate)
		{
			pSchemaUpdate->Release();
		}

		if (pwszTemp)
		{
			pwszTemp = NULL;
			delete pwszTemp;
		}

		return S_OK;
	}
}

// main
int _tmain(int argc, _TCHAR* argv[])
{
	HRESULT hr;
	IADs    *pRoot = NULL;
	VARIANT varDSRoot, 
			varSchemaUpdate;
	LPWSTR  pwszDSPath = NULL;

	// Returned by CreateDSObject
	IDispatch *pDisp = NULL;

	// Pointers to schema objects
	IDirectoryObject    *pSchema = NULL;

	UINT i = 0, jCount = 0;

	LPWSTR TempString = new WCHAR[128];
	LPWSTR CONF_FILE = new WCHAR[128];

	if (argc < 2)
	{
		wmemset( CONF_FILE, 0x00, 128 );
		lstrcpyW(CONF_FILE, L"ad.conf");
	}
	else if (argc == 3)
	{
		wmemset( TempString, 0x00, 128 );
		MultiByteToWideChar(CP_ACP,
			0,
			argv[1],
			3,
			TempString,
			3);
		TempString[3] = 0;

		if ((!lstrcmpW(TempString, L"/p:")) || (!lstrcmpW(TempString, L"/P:")) ||
			(!lstrcmpW(TempString, L"-p:")) || (!lstrcmpW(TempString, L"-P:")))
		{
			getTCPPort(&argv[1][3]);

			MultiByteToWideChar(CP_ACP,
				0,
				argv[2],
				lstrlen(argv[2]) + 1,
				CONF_FILE,
				128);
		}
		else
		{
			wmemset( TempString, 0x00, 128 );
			MultiByteToWideChar(CP_ACP,
				0,
				argv[2],
				3,
				TempString,
				3);
			TempString[3] = 0;

			if ((!lstrcmpW(TempString, L"/p:")) || (!lstrcmpW(TempString, L"/P:")) ||
				(!lstrcmpW(TempString, L"-p:")) || (!lstrcmpW(TempString, L"-P:")))
			{
				getTCPPort(&argv[2][3]);

				MultiByteToWideChar(CP_ACP,
					0,
					argv[1],
					lstrlen(argv[1]) + 1,
					CONF_FILE,
					128);
			}
			else
			{
				ShowUsage();
				goto cleanup;
			}
		}
	}
	else
	{
		wmemset( TempString, 0x00, 128 );
		MultiByteToWideChar(CP_ACP,
			0,
			argv[1],
			2,
			TempString,
			2);
		TempString[2] = 0;

		if ((!lstrcmpW(TempString, L"/h")) || (!lstrcmpW(TempString, L"/H")) ||
			(!lstrcmpW(TempString, L"-h")) || (!lstrcmpW(TempString, L"-H")))
		{
			ShowHelp();
			goto cleanup;
		}
		else if ((!lstrcmpW(TempString, L"/p")) || (!lstrcmpW(TempString, L"/P")) ||
			(!lstrcmpW(TempString, L"-p")) || (!lstrcmpW(TempString, L"-P")))
		{
			getTCPPort(&argv[1][3]);

			lstrcpyW(CONF_FILE, L"ad.conf");
		}
		else if ((argv[1][0] == '/') || (argv[1][0] == '-'))
		{
			ShowUsage();
			goto cleanup;
		}
		else
		{
			// filename only
			MultiByteToWideChar(CP_ACP,
				0,
				argv[1],
				lstrlen(argv[1]) + 1,
				CONF_FILE,
				128);
		}
	}

	// Show Logo
	ShowLogo();

	// Show Configuration file
	wprintf(L"Configuration file: %s\n", CONF_FILE);

	// ATTR_INFO for creating an attributeSchema object
	// Each ADS_ATTR_INFO describes one attribute of an object to be
	// stored in the DS.

	ADS_ATTR_INFO   attrArray[] = {
		{L"cn",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&cn,1},
		{L"isSingleValued",ADS_ATTR_UPDATE,ADSTYPE_BOOLEAN,&singleValued,1},
		{L"objectClass",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&objectClass,1},
		{L"attributeID",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&oid,1},
		{L"attributeSyntax",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&syntax,1},
		{L"oMSyntax",ADS_ATTR_UPDATE,ADSTYPE_INTEGER,&omSyntax,1},
		//--> do not need for defining an attribute.
		{L"lDAPDisplayName",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&ldapname,1},
		//{L"schemaIdGUID",ADS_ATTR_UPDATE,ADSTYPE_OCTET_STRING,&idGuid,1},
	};

	DWORD           dwAttrs;

	::CoInitialize(NULL);
	//hr = CoInitializeEx(NULL, COINIT_MULTITHREADED | COINIT_DISABLE_OLE1DDE);

	//  Get the name of the schema container for this domain. 
	//  Read the Root DSE from the default DS, which will be the DS for 
	//  the local domain. This will get the name of the schema container,
	//  stored in the "schemaNamingContext" operational attribute.
	if (tcpPort != 389)
	{
		wmemset(TempString, 0x00, sizeof(TempString));
		swprintf(TempString, L"LDAP://localhost:%lu/RootDSE", tcpPort);
		hr = ADsGetObject(TempString,
							IID_IADs,
							(void**)&pRoot);
	}
	else
	{
		hr = ADsGetObject(L"LDAP://localhost/RootDSE",
							IID_IADs,
							(void**)&pRoot);
	}
	if (FAILED(hr))
	{
		wprintf(L"LDAP Schema updates enabled DC is needed to execute this program.\n");
		goto cleanup;
	}

	// Get IDirectoryObject on the root DSE as well; use this for 
	// forcing a schema update later.
	VariantInit(&varDSRoot);
	hr = pRoot->Get(CComBSTR("schemaNamingContext"), &varDSRoot);
	if (FAILED(hr))
	{
		wprintf(L"LDAP schemaNamingContext can not be used.\n");
		goto cleanup;
	}
	//wprintf(L"\nDS Root: %s\n", varDSRoot.bstrVal);

	// ADsPath of the schema container
	pwszDSPath = new WCHAR[32 + lstrlenW(varDSRoot.bstrVal) + 1];
	if (!pwszDSPath)
		goto cleanup;

	wmemset( pwszDSPath, 0x00, 32 + lstrlenW(varDSRoot.bstrVal) + 1 );

	if (tcpPort != 389)
		swprintf(pwszDSPath, L"LDAP://localhost:%lu/", tcpPort);
	else
		lstrcpyW(pwszDSPath, L"LDAP://localhost/");

	lstrcatW(pwszDSPath, varDSRoot.bstrVal);
	//wprintf(L"\nDS Path: %s\n", pwszDSPath);

	// Bind to the schema container and get the IDirectoryObject interface
	// on it.
	hr = ADsGetObject(pwszDSPath,
						IID_IDirectoryObject,
						(void**)&pSchema);
	if(FAILED(hr))
	{
		wprintf(L"LDAP binding failed!\n");
		goto cleanup;
	}

	// Show DS Path
	wprintf(L"DS Path: %s\n\n", pwszDSPath);

	// Get Attributes & Classes from configuration file.
	if (!getConfig(CONF_FILE))
	{
		wprintf(L"Configuration Get failed!\n");
		goto cleanup;
	}

	// count the number of mayContain
	for (i = 0; i < (sizeof(bufArray) / 7 / 256 / 2); i++)
	{
		if (bufArray[i][0][0] == '\0')
		{
			break;
		}

		if (lstrcmpW(bufArray[i][0], L"ATR") || (bufArray[i][3][0] == '\0'))
		{
			break;
		}
	}

	try
	{
		// define mayContain array dynamically
		ADSVALUE	*mayContain;
		mayContain = new ADSVALUE[i];

		// ATTR_INFO for creating a classSchema object
		ADS_ATTR_INFO   classArray[] = {
			{L"cn",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&cn,1},
			{L"objectClass",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&objectClass,1},
			{L"governsID",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&oid,1},
			{L"objectClassCategory",ADS_ATTR_UPDATE,ADSTYPE_INTEGER,&objectClassCategory,1},
			//{L"schemaIdGUID",ADS_ATTR_UPDATE,ADSTYPE_OCTET_STRING,&idGuid,1},
			{L"defaultSecurityDescriptor",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&defaultSecurityDesc,1},
			{L"defaultHidingValue",ADS_ATTR_UPDATE,ADSTYPE_BOOLEAN,&defaultHidingValue,1},
			{L"subClassOf",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&subClassOf,1},
			{L"mayContain",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&mayContain[0],i},
		};

		ADS_ATTR_INFO   na_classArray[] = {
			{L"cn",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&cn,1},
			{L"objectClass",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&objectClass,1},
			{L"governsID",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&oid,1},
			{L"objectClassCategory",ADS_ATTR_UPDATE,ADSTYPE_INTEGER,&objectClassCategory,1},
			//{L"schemaIdGUID",ADS_ATTR_UPDATE,ADSTYPE_OCTET_STRING,&idGuid,1},
			{L"defaultSecurityDescriptor",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&defaultSecurityDesc,1},
			{L"defaultHidingValue",ADS_ATTR_UPDATE,ADSTYPE_BOOLEAN,&defaultHidingValue,1},
			{L"subClassOf",ADS_ATTR_UPDATE,ADSTYPE_CASE_IGNORE_STRING,&subClassOf,1},
		};

		// ATTR_INFO for adding attributes to Class
		ADS_ATTR_INFO schemaUpdate[] = {
			{L"mayContain",ADS_ATTR_APPEND,ADSTYPE_CASE_IGNORE_STRING,&mayContain[0],i},
		};

		for (i = 0; i < (sizeof(bufArray) / 7 / 256 / 2); i++)
		{
			if (bufArray[i][0][0] == '\0')
			{
				break;
			}

			if (!lstrcmpW(bufArray[i][0], L"ATR"))
			{
				GUID *guid = NULL;
				//GUID *guid = &getGUID(bufArray[i][6]);	// GUID is auto-generated.

				dwAttrs = sizeof(attrArray)/sizeof(ADS_ATTR_INFO); // Calculate attribute count

				// OID is null; add to class & skip creation
				if (bufArray[i][4][0] == '\0')
				{
					mayContain[jCount].dwType = ADSTYPE_CASE_IGNORE_STRING;
					mayContain[jCount++].CaseIgnoreString = (ADS_CASE_IGNORE_STRING) bufArray[i][3];

					continue;
				}

				hr = CreateAttribute(bufArray[i][1], bufArray[i][2], bufArray[i][3], 
					bufArray[i][4], bufArray[i][5],
					attrArray, dwAttrs, /* (*guid), */ pDisp, pSchema);
				if (FAILED(hr))
				{
					// already exists; add to class & skip update
					if (hr == 0x80071392)
					{
						mayContain[jCount].dwType = ADSTYPE_CASE_IGNORE_STRING;
						mayContain[jCount++].CaseIgnoreString = (ADS_CASE_IGNORE_STRING) bufArray[i][3];

						continue;
					}
					else
					{
						break;
					}
				}

				mayContain[jCount].dwType = ADSTYPE_CASE_IGNORE_STRING;
				mayContain[jCount++].CaseIgnoreString = (ADS_CASE_IGNORE_STRING) bufArray[i][3];

				// Force an update of the schema cache to create the class that includes
				// these attributes. Force a synchronous schema update by writing
				// the operational attribute "schemaUpdateNow" to the Root DSE.
				//
				varSchemaUpdate.vt = VT_I4;
				varSchemaUpdate.intVal = 1;
				hr = pRoot->Put(CComBSTR("schemaUpdateNow"), varSchemaUpdate);
				hr = pRoot->SetInfo();
				if (FAILED(hr)) 
				{
					ReportErrorW(L"Force Schema Recalc failed.", hr);
					break;
				}
			}
			else if (!lstrcmpW(bufArray[i][0], L"CLS"))
			{
				// no attribute to be added
				if (jCount == 0)
				{
					dwAttrs = sizeof(na_classArray)/sizeof(ADS_ATTR_INFO); // Calculate attribute count

					// OID is null; skip class creation & updation all
					if (bufArray[i][2][0] == '\0')
					{
						wprintf(L"Update class is not avaliable.\n");
					}
					else
					{
						hr = CreateClass(bufArray[i][1], bufArray[i][2], bufArray[i][3],
							classArray, dwAttrs, pDisp, pSchema);
						if (FAILED(hr))
						{
							ReportErrorW(L"Force Schema Update for creating a class failed.", hr);
							break;
						}
					}
				}
				else
				{
					dwAttrs = sizeof(classArray)/sizeof(ADS_ATTR_INFO); // Calculate attribute count

					// OID is null; skip class creation & update it
					if (bufArray[i][2][0] == '\0')
					{
						// show adding attributes to class
						wprintf(L"\nAdding attibutes to class %s.\n", bufArray[i][1]);
						for (UINT ii = 0; ii < jCount; ii++)
						{
							wprintf(L"    Attibute: %s\n", mayContain[ii].CaseIgnoreString);
						}

						hr = UpdateClass(bufArray[i][1], varDSRoot.bstrVal, schemaUpdate);
						if (FAILED(hr))
						{
							ReportErrorW(L"Force Schema Update for adding attributes failed.", hr);
							break;
						}
						else
						{
							continue;
						}
					}
					else
					{
						// show adding attributes to class
						wprintf(L"\nAdding attibutes to class %s.\n", bufArray[i][1]);
						for (UINT ii = 0; ii < jCount; ii++)
						{
							wprintf(L"    Attibute: %s\n", mayContain[ii].CaseIgnoreString);
						}

						hr = CreateClass(bufArray[i][1], bufArray[i][2], bufArray[i][3],
							classArray, dwAttrs, pDisp, pSchema);
						if (FAILED(hr))
						{
							if ((hr == 0x8007202F) || (hr = 0x80071392))	// already exists : access violation
							{
								hr = UpdateClass(bufArray[i][1], varDSRoot.bstrVal, schemaUpdate);
								if (FAILED(hr))
								{
									ReportErrorW(L"Force Schema Update for adding attributes failed.", hr);
									break;
								}
								else
								{
									continue;
								}
							}
							else
							{
								break;
							}
						}
					}
				}

				// Force an update of the schema cache to use the changes immediately.
				// Force a synchronous schema update by writing the operational
				// attribute "schemaUpdateNow" to the Root DSE.
				//
				varSchemaUpdate.vt = VT_I4;
				varSchemaUpdate.intVal = 1;
				hr = pRoot->Put(CComBSTR("schemaUpdateNow"), varSchemaUpdate);
				hr = pRoot->SetInfo();
				if (FAILED(hr)) 
				{
					ReportErrorW(L"Force Schema Recalc failed.", hr);
					break;
				}
			}
		}
	}
	catch (...)
	{
		ReportErrorW(L"Unknown Error", GetLastError());
	}

cleanup:
	VariantClear(&varDSRoot);
	VariantClear(&varSchemaUpdate);

	if (pwszDSPath)
	{
		pwszDSPath = NULL;
		delete pwszDSPath;
	}

	if(pDisp)
	{
		pDisp->Release();
	}

	if(pSchema)
	{
		pSchema->Release();
	}

	if(pRoot)
	{
		pRoot->Release();
	}

	if (TempString)
	{
		TempString = NULL;
		delete TempString;
	}

	if (CONF_FILE)
	{
		CONF_FILE = NULL;
		delete CONF_FILE;
	}

	::CoUninitialize();
	return 0;
}


// Read from configuration file
bool getConfig(LPCWSTR CONF_FILE)
{
	int fd;
	WCHAR c[2] = L"\0";
	WCHAR buffer[256];
	memset(buffer, 0x00, sizeof(buffer));
	UINT i = 0, pos = 0, j = 0, k = 0;
	bool bExit = false;

	if ((fd = _wopen(CONF_FILE, _O_RDONLY, _S_IREAD)) < 0)
	{
		wprintf(L"Can not open configuration file: %s\n", CONF_FILE);
		return false;
	}
	memset(bufArray, 0x00, sizeof(bufArray));

	while (!bExit)
	{
		while (true)
		{
			int nRead = _read(fd, &c, 1);
			if (nRead <= 0)
			{
				bExit = true;
				break;
			}

			if (c[0] == (WCHAR) 10)
			{
				if ((buffer[0] == '#') || (buffer[0] == '\0') || (buffer[0] == ' '))
				{
					memset(buffer, 0x00, sizeof(buffer));
					i = 0;
				}
				else
				{
					break;
				}
			}
			else
			{
				buffer[i] = c[0];
				i++;
			}
		}

		if (buffer[0] != NULL)
		{
			memset(bufArray[k][0], 0x00, sizeof(bufArray[k][0]));
			for (i = 0; i < wcslen(buffer); i++)
			{
				if (buffer[i] == ',')
				{
					bufArray[k][j][pos] = '\0';

					pos = 0;
					j++;
					memset(bufArray[k][j], 0x00, sizeof(bufArray[k][j]));
				}
				else
				{
					bufArray[k][j][pos++] = buffer[i];
				}
			}
			bufArray[k][j][pos] = '\0';

			k++;
			j = 0;
			i = 0;
			pos = 0;
			memset(buffer, 0x00, sizeof(buffer));
		}
	}
	_close(fd);

	return true;
}


// get GUID from String
GUID getGUID(WCHAR* pwszGuid)
{
	DWORD digit1, digit2, digit3;
	DWORD ux1, ux2, ux3, ux4, ux5, ux6, ux7, ux8;
	UINT i = 0, pos = 0, j = 0;

	DWORD dwTemp;
	WCHAR *sTemp;
	WCHAR *buffer[5];

	GUID guidTemp;

	buffer[0] = new WCHAR[32];
	wmemset( buffer[0], 0x00, 32 );

	for (i = 0; i < wcslen(pwszGuid); i++)
	{
		if (pwszGuid[i] == '-')
		{
			buffer[j][pos] = '\0';

			pos = 0;
			j++;
			buffer[j] = new WCHAR[512];
			wmemset( buffer[j], 0x00, 512 );
		}
		else
		{
			buffer[j][pos++] = pwszGuid[i];
		}
	}
	buffer[j][pos] = '\0';

	sTemp = new WCHAR[1024];
	wmemset( sTemp, 0x00, 1024 );

	// 1st digit processing...
	dwTemp = 0;
	memcpy(sTemp, buffer[0], 4);
	swscanf(sTemp, L"%2x", &digit1);
	dwTemp = digit1 * 0x01000000;

	memcpy(sTemp, &buffer[0][2], 4);
	swscanf(sTemp, L"%2x", &digit1);
	dwTemp += digit1 * 0x010000;

	memcpy(sTemp, &buffer[0][4], 4);
	swscanf(sTemp, L"%2x", &digit1);
	dwTemp += digit1 * 0x0100;

	memcpy(sTemp, &buffer[0][6], 4);
	swscanf(sTemp, L"%2x", &digit1);
	dwTemp += digit1;
	digit1 = dwTemp;

	// 2nd digit processing...
	dwTemp = 0;
	memcpy(sTemp, buffer[1], 4);
	swscanf(sTemp, L"%2x", &digit2);
	dwTemp = digit2 * 0x0100;

	memcpy(sTemp, &buffer[1][2], 4);
	swscanf(sTemp, L"%2x", &digit2);
	dwTemp += digit2;
	digit2 = dwTemp;

	// 3rd digit processing...
	dwTemp = 0;
	memcpy(sTemp, buffer[2], 4);
	swscanf(sTemp, L"%2x", &digit3);
	dwTemp = digit3 * 0x0100;

	memcpy(sTemp, &buffer[2][2], 4);
	swscanf(sTemp, L"%2x", &digit3);
	dwTemp += digit3;
	digit3 = dwTemp;

	// 4th digit processing...
	swscanf(buffer[3], L"%2x%2x", &ux1, &ux2);
	swscanf(buffer[4], L"%2x%2x%2x%2x%2x%2x", &ux3, &ux4, &ux5, &ux6, &ux7, &ux8);

	memset(&guidTemp, 0x00, sizeof(guidTemp));
	guidTemp.Data1 = digit1;
	guidTemp.Data2 = (USHORT) digit2;
	guidTemp.Data3 = (USHORT) digit3;
	guidTemp.Data4[0] = (UCHAR) ux1;
	guidTemp.Data4[1] = (UCHAR) ux2;
	guidTemp.Data4[2] = (UCHAR) ux3;
	guidTemp.Data4[3] = (UCHAR) ux4;
	guidTemp.Data4[4] = (UCHAR) ux5;
	guidTemp.Data4[5] = (UCHAR) ux6;
	guidTemp.Data4[6] = (UCHAR) ux7;
	guidTemp.Data4[7] = (UCHAR) ux8;

	for (i = 0; i < 5; i++)
	{
		delete[] buffer[i];
	}
	delete sTemp;

	return guidTemp;
}

void getTCPPort(_TCHAR* argv)
{
	ULONG iTemp = 0;
	sscanf(argv, "%lu", &iTemp);
	if (iTemp > 0)
		tcpPort = iTemp;
}

// Simple error message reporter
void ReportErrorW(LPCWSTR pwszDefaultMsg, DWORD dwErr)
{
	DWORD   dwStatus;
	LPTSTR  lpszMsg;

	dwStatus = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM |
								FORMAT_MESSAGE_ALLOCATE_BUFFER |
								FORMAT_MESSAGE_IGNORE_INSERTS,
								NULL,
								dwErr,
								LANG_NEUTRAL,
								(LPTSTR)&lpszMsg,
								64,
								NULL); 
	if (dwStatus != 0) 
	{
		if (lpszMsg != NULL)
		{
			wprintf(L"%s[0x%X]: %S", pwszDefaultMsg, dwErr, lpszMsg);
		}
		else
		{
			wprintf(L"%s[0x%X]\n", pwszDefaultMsg, dwErr);
		}
		LocalFree(lpszMsg);
	} 
	else
	{
		wprintf(L"%s[0x%X]\n", pwszDefaultMsg, dwErr);
	}
}

void ShowUsage()
{
	wprintf(L"Type 'SchemaCreator.exe { /h | -h }' for help.\n");
}

void ShowHelp()
{
	ShowLogo();

	wprintf(L"Usage:\n");
	wprintf(L"SchemaCreator.exe [[ /p:PORT | -p:PORT ] | [ CONF_FILE_PATH ]] | [ /h | -h ]\n\n");
	wprintf(L"Options:\n");
	wprintf(L"  {none}            : Execute using default configuration file(\"ad.conf\").\n");
	wprintf(L"  /h | -h           : Show this help messages\n");
	wprintf(L"  /p:PORT | -p:PORT : Connect to DC using specified TCP Port number\n");
	wprintf(L"  CONF_FILE_PATH    : Execute using given configuration file.\n\n");
	wprintf(L"Notes:      - Must be run on a DC which is the current Schema Master and has\n");
	wprintf(L"              schema updates enabled using the registry key and value:\n\n");
	wprintf(L"              KEY:  HKLM\\CurrentControlSet\\Services\\NTDS\\Parameters\n");
	wprintf(L"              Value:    Schema Update Allowed, REG_DWORD, 1\n\n");
	wprintf(L"            - This version works on ADAM also.\n");
}

void ShowLogo()
{
	wprintf(L"ADSC (Active Directory Schema Creator) version 1.5b\n");
	wprintf(L"--------------------------------------------------\n");
	wprintf(L"-- Written by thermidor (thermidor@nets.co.kr)\n");
	wprintf(L"-- Last updated on 25 Mar, 2004\n\n");
}
