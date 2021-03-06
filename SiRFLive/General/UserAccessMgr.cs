﻿namespace SiRFLive.General
{
    using System;
    using System.Collections;
    using System.Xml;

    public class UserAccessMgr
    {
        private XmlDocument _userAccessCfgXMLDoc;
        private string _XmlFile;

        public UserAccessMgr(string xmlFile)
        {
            try
            {
                _XmlFile = xmlFile;
                _userAccessCfgXMLDoc = new XmlDocument();
                _userAccessCfgXMLDoc.Load(xmlFile);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public string GetAccessNum()
        {
            string str = "";
            XmlNode node = _userAccessCfgXMLDoc.SelectSingleNode("/UserAccess/accessNum");
            try
            {
                str = node.Attributes["value"].Value;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str;
        }

        public string GetCurrentUser()
        {
            string str = "";
            XmlNode node = _userAccessCfgXMLDoc.SelectSingleNode("/UserAccess/currentUser");
            try
            {
                str = node.Attributes["name"].Value;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str;
        }

        public Hashtable GetUserAccessInfo(string username)
        {
            Hashtable hashtable = new Hashtable();
            XmlNodeList list = _userAccessCfgXMLDoc.SelectNodes("/UserAccess/accesslevel/user[@name='" + username + "'][@subid = '1']/menu");
            try
            {
                foreach (XmlNode node in list)
                {
                    hashtable.Add(node.Attributes["name"].Value, node.Attributes["access"].Value);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return hashtable;
        }
    }
}

