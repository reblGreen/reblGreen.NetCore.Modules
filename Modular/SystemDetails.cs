/*
    The MIT License (MIT)

    Copyright (c) 2015  The Modular Project (https://bitbucket.org/juanshaf/modular)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Modular
{
    public class SystemDetails
    {
        private Regex ipRegex = new Regex(@"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)", RegexOptions.Compiled);
        private Regex ipv6Regex = new Regex(@"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))", RegexOptions.Compiled);

        public string MachineName { get; private set; }
        public string MacAddress { get; private set; }
        public string LocalIPAddress { get; private set; }
        public string PublicIPAddress { get; private set; }
        public string FQDN { get; private set; }

        public SystemDetails()
        {
            this.MachineName = GetMachineName();
            this.MacAddress = GetMacAddress();
            this.LocalIPAddress = GetLocalIPAddress();
            this.PublicIPAddress = GetPublicIPAddress();
            this.FQDN = GetFQDN();
        }

        private string GetFQDN()
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

            if (ipProperties != null)
            {
                return string.IsNullOrWhiteSpace(ipProperties.DomainName) ? ipProperties.HostName : string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
            }

            return null;
        }

        private string GetMachineName()
        {
            return Environment.MachineName;
        }

        private string GetLocalIPAddress()
        {
            foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return null;
        }

        private string GetPublicIPAddress()
        {
            string[] ipServers = new string[]
            {
                "https://api.mediaskunkworks.com/my-ip",
                "http://ifcfg.me/i",
                "http://myip.dnsomatic.com/",
                "http://ifconfig.me/ip",
                "http://curlmyip.com/",
                "http://myexternalip.com/raw",
                "http://checkip.dyndns.org/",
                "http://whatismyip.org/"
            };

            using (WebClient wc = new WebClient())
            {
                foreach (string s in ipServers)
                {
                    try
                    {
                        string context = wc.DownloadString(s);
                        string ip = ipRegex.Match(context).Value;

                        if (!string.IsNullOrEmpty(ip))
                        {
                            return ip;
                        }

                        ip = ipv6Regex.Match(context).Value;

                        if (!string.IsNullOrEmpty(ip))
                        {
                            return ip;
                        }
                    }
                    catch { }
                }
            }

            return null;
        }

        private string GetMacAddress()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            if (interfaces.Length > 0)
            {
                byte[] macAddr = interfaces[0].GetPhysicalAddress().GetAddressBytes();

                string s = "";
                for (int i = 0; i < macAddr.Length; i++)
                {
                    s += string.Format("{0:x2}", macAddr[i]) + (i < macAddr.Length - 1 ? ":" : "");
                }

                return s.ToUpperInvariant();
            }
            return null;
        }

        public override string ToString()
        {
            string s = "Machine Name: " + this.MachineName;
            s += !string.IsNullOrEmpty(this.MacAddress) ? ". MAC Address: " + this.MacAddress : "";
            s += !string.IsNullOrEmpty(this.LocalIPAddress) ? ". Local IP Address: " + this.LocalIPAddress : "";
            s += !string.IsNullOrEmpty(this.PublicIPAddress) ? ". Public IP Address: " + this.PublicIPAddress : "";
            s += !string.IsNullOrEmpty(this.FQDN) ? ". Fully Qualified Domain Name: " + this.FQDN : "";
            s += ".";

            return s;
        }
    }
}
