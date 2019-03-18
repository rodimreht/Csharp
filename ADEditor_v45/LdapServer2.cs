using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ADEditor
{
    public class LdapServer2
    {
        private LdapConnection _ldapConnection;
        private string _baseDN;

        public bool Connect(string ip, int port, string user, string password, string baseDN)
        {
            try
            {
                // Create the new LDAP connection
                LdapDirectoryIdentifier ldi = new LdapDirectoryIdentifier(ip, port);
                _ldapConnection = new LdapConnection(ldi);
                _ldapConnection.AuthType = AuthType.Ntlm;
                _ldapConnection.SessionOptions.ProtocolVersion = 3;
                _ldapConnection.SessionOptions.Sealing = true;

                NetworkCredential nc = new NetworkCredential(user, password);
                _ldapConnection.Bind(nc);

                _baseDN = baseDN;
                return true;
            }
            catch (LdapException e)
            {
                Debug.Write("Unable to login: " + e.Message);
            }
            catch (Exception e)
            {
                Debug.Write("Unexpected exception occurred: " + e.GetType() + ":" + e.Message);
            }
            return false;
        }

        public bool SetProperty(string targetDN, string name, string value)
        {
            DirectoryAttributeModification mod = new DirectoryAttributeModification();
            mod.Operation = DirectoryAttributeOperation.Replace;
            mod.Name = name;
            mod.Add(value);

            ModifyRequest modifyRequest = new ModifyRequest(targetDN, mod);
            DirectoryResponse response = _ldapConnection.SendRequest(modifyRequest);
            Debug.Write(response.ResultCode);
            return (response.ResultCode == ResultCode.Success);
        }

        public bool ChangePassword(string targetDN, string oldPassword, string newPassword)
        {
            DirectoryAttributeModification deleteMod = new DirectoryAttributeModification();
            deleteMod.Name = "unicodePwd";
            deleteMod.Add(GetPasswordData(oldPassword));
            deleteMod.Operation = DirectoryAttributeOperation.Delete;

            DirectoryAttributeModification addMod = new DirectoryAttributeModification();
            addMod.Name = "unicodePwd";
            addMod.Add(GetPasswordData(newPassword));
            addMod.Operation = DirectoryAttributeOperation.Add;

            ModifyRequest request = new ModifyRequest(targetDN, deleteMod, addMod);

            DirectoryResponse response = _ldapConnection.SendRequest(request);
            Debug.Write(response.ResultCode);
            return (response.ResultCode == ResultCode.Success);
        }

        public bool SetPassword(string targetDN, string password)
        {
            DirectoryAttributeModification pwdMod = new DirectoryAttributeModification();
            pwdMod.Name = "unicodePwd";
            pwdMod.Add(GetPasswordData(password));
            pwdMod.Operation = DirectoryAttributeOperation.Replace;

            ModifyRequest request = new ModifyRequest(targetDN, pwdMod);

            DirectoryResponse response = _ldapConnection.SendRequest(request);
            Debug.Write(response.ResultCode);
            return (response.ResultCode == ResultCode.Success);
        }

        private static byte[] GetPasswordData(string password)
        {
            string formattedPassword;
            formattedPassword = String.Format("\"{0}\"", password);
            return (Encoding.Unicode.GetBytes(formattedPassword));
        }

        /// <summary>
        /// Pageds the search.
        /// </summary>
        /// <param name="searchFilter">The search filter.</param>
        /// <param name="attributesToLoad">The attributes to load.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        /// <remarks>
        /// var searchFilter = "nextUID=*";
        /// var attributesToLoad = new[] {"nextUID"};
        /// var pagedSearchResults = PagedSearch(
        ///     searchFilter,
        ///     attributesToLoad);
        ///
        /// foreach (var searchResultEntryCollection in pagedSearchResults)
        ///     foreach (SearchResultEntry searchResultEntry in searchResultEntryCollection)
        ///         Console.WriteLine(searchResultEntry.Attributes["nextUID"][0]);
        /// </remarks>
        public IEnumerable<SearchResultEntryCollection> PagedSearch(string searchFilter, string[] attributesToLoad, int pageSize = 1000)
        {
            SearchRequest searchRequest = new SearchRequest(_baseDN, searchFilter, SearchScope.Subtree, attributesToLoad);
            SearchOptionsControl searchOptions = new SearchOptionsControl(SearchOption.DomainScope);
            searchRequest.Controls.Add(searchOptions);

            var pageResultRequestControl = new PageResultRequestControl(pageSize);
            searchRequest.Controls.Add(pageResultRequestControl);

            while (true)
            {
                SearchResponse searchResponse = (SearchResponse)_ldapConnection.SendRequest(searchRequest);
                PageResultResponseControl pageResponse = (PageResultResponseControl)searchResponse.Controls[0];

                yield return searchResponse.Entries;
                if (pageResponse.Cookie.Length == 0) break;

                pageResultRequestControl.Cookie = pageResponse.Cookie;
            }
        }

        public SearchResultEntry GetDirectoryEntry(string searchFilter, string[] attributesToLoad)
        {
            SearchRequest searchRequest = new SearchRequest(_baseDN, searchFilter, SearchScope.Subtree, attributesToLoad);
            SearchOptionsControl searchOptions = new SearchOptionsControl(SearchOption.DomainScope);
            searchRequest.Controls.Add(searchOptions);

            SearchResponse searchResponse = (SearchResponse)_ldapConnection.SendRequest(searchRequest);
            return searchResponse.Entries[0];
        }
    }
}
