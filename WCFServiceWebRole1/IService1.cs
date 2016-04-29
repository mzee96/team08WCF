using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WCFServiceWebRole1;

namespace WCFServiceBonHotels1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string Login(string username, string password);

        [OperationContract]
        string insertUser(User newUser);

        [OperationContract]
        string changeUserType(string username, int type);

        [OperationContract]
        List<User> getUserList();

        [OperationContract]
        List<User> getUser(string username);

        [OperationContract]
        string updateUser(User user);

        [OperationContract]
        string getUsername(string email);
    }
}
