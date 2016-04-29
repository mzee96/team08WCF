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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        EventPlanning_LINQDataContext db = new EventPlanning_LINQDataContext();
        User user = new User();
        Customer customer = new Customer();
        Admin admin = new Admin();
        Employee employee = new Employee();
        Planner planner = new Planner();


        /**
          * Used for adding a new user into the system depending on the type
          * param User="newUser" - of type User
          * returns adding user status whether exits of success
        **/
        public string insertUser(User newUser)
        {
            var u = from user in db.Users
                    select user;

            foreach (User user in u)
            {
                if (user.Email == newUser.Email)
                {
                    return "Exists";
                }
            }

            User newDBUser = newUser;
            newDBUser.Username = newUser.Email;
            db.Users.InsertOnSubmit(newDBUser);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            //Customer customer = new Customer()
            //{
            //    CusID = newDBUser.UserID
            //};

            if (newDBUser.Type == 3)
            {
                employee.UserID = newUser.UserID;
                db.Employees.InsertOnSubmit(employee);
            }
            if (newDBUser.Type == 1)
            {
                admin.UserID = newUser.UserID;
                db.Admins.InsertOnSubmit(admin);
            }
            if (newDBUser.Type == 4)
            {
                planner.UserID = newUser.UserID;
                db.Planners.InsertOnSubmit(planner);
            }
            else
            {
                customer.UserID = newDBUser.UserID;
                db.Customers.InsertOnSubmit(customer);
            }


            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                return "Submit error: " + "\n" + e.ToString();
            }

            return "Success";
        }

        /**
         * Used for login into the system
         * param username="username"
         * param password="password"
         * returns usertype as string or no match if not found 
         **/
        public string Login(string username, string password)
        {
            var response = "No match";
            var u = from user in db.Users
                    select user;

            foreach (User user in u)
            {
                if (user.Password == password)
                {
                    if (user.Email == username)
                    {
                        switch (user.Type)
                        {
                            case 1:
                                response = "Admin";
                                break;
                            case 2:
                                response = "Customer";
                                break;
                            case 3:
                                response = "Employee";
                                break;
                            case 4:
                                response = "Planner";
                                break;
                            default:
                                response = "Unknown";
                                break;
                        }
                    }
                }
            }
            return response;
        }

        /**
         * can only be requested by an admin
         * returns a list of all users of type User
         * */
        public List<User> getUserList()
        {
            var query = from user in db.Users
                        select user;

            List<User> result = new List<User>();


            foreach (User user in query)
            {
                User currUser = new User();
                currUser.Name = user.Name;
                currUser.Surname = user.Surname;
                currUser.UserID = user.UserID;
                currUser.Username = user.Username;
                currUser.Type = user.Type;
                currUser.RegDate = user.RegDate;
                currUser.Gender = user.Gender;
                currUser.Contact = user.Contact;

                result.Add(currUser);

            }
            return result;
        }

        /**
         * availability admin
         * param = "username" = username to identify requested user
         * return a requested user (User) from the database and the details
         * */
        public List<User> getUser(string username)
        {
            var query = from user in db.Users
                        where user.Username == username
                        select user;
            List<User> result = new List<User>();

            User currUser = new User();

            foreach (User user in query)
            {
                currUser.Name = user.Name;
                currUser.Surname = user.Surname;
                currUser.UserID = user.UserID;
                currUser.Username = user.Username;
                currUser.Type = user.Type;
                currUser.RegDate = user.RegDate;
                currUser.Gender = user.Gender;
                currUser.Contact = user.Contact;

                result.Add(currUser);
            }
            return result;
        }

        //public string updateUser(int userID, String username, String firstName, String lastName, String Gender, String email, String contact)
        //{
        //    var query = from user in db.Users
        //                where user.UserID == userID
        //                select user;

        //    foreach (User users in query)
        //    {
        //        users.Username = username;
        //        users.Name = firstName;
        //        users.Surname = lastName;
        //        users.Gender = Gender;
        //        users.Email = email;
        //        users.Contact = contact;
        //    }
        //    try
        //    {
        //        db.SubmitChanges();
        //    }
        //    catch (Exception exception)
        //    {
        //        return exception.Message;
        //    }
        //    return "Successful";
        //}





        /**
         * 
         * param userUpdate="userUpdate" of type User
         * return a requested user (User) from the database and the details
         * */
        public string updateUser(User userUpdate)
        {

            var query = from user in db.Users
                        where user.UserID == userUpdate.UserID
                        select user;

            foreach (User users in query)
            {
                users.Username = userUpdate.Username;
                users.Name = userUpdate.Name;
                users.Surname = userUpdate.Surname;
                users.Gender = userUpdate.Gender;
                users.Email = userUpdate.Email;
                users.Contact = userUpdate.Contact;
            }
            try
            {
                db.SubmitChanges();
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return "Successful";
        }

        /**
         * Availability - Admin
          * Used for changing user type 
          * param username="username"
          * param type="type" - of type int
          * returns message whether successul or failed
        **/
        public string changeUserType(string username, int type)
        {
            var response = "";
            var tempUserID = 0;
            var tempUserType = 0;
            var query = from user in db.Users
                        where user.Username == username
                        select user;
            
            foreach (User users in query)
            {
                tempUserType = (int) users.Type;
                tempUserID = users.UserID;
                users.Type = type; 
            }

            //delete user type from original table
            switch (tempUserType)
            {
                case 1:
                    var q = from admin in db.Admins
                            where admin.UserID == tempUserID
                            select admin;
                    
                    foreach (Admin admin in q)
                    {
                        db.Admins.DeleteOnSubmit(admin);
                    }
               
                    break;
                case 2:

                    var q2 = from cus in db.Customers
                            where cus.UserID == tempUserID
                            select cus;

                    foreach (Customer customer in q2)
                    {
                        db.Customers.DeleteOnSubmit(customer);
                    }
                    break;
                case 3:

                    var q3 = from employee in db.Employees
                            where employee.UserID == tempUserID
                            select employee;

                    foreach (Employee empl in q3)
                    {
                        db.Employees.DeleteOnSubmit(empl);
                    }
                    break;

                case 4:

                    var q4 = from planner in db.Planners
                            where planner.UserID == tempUserID
                            select planner;

                    foreach (Planner plann in q4)
                    {
                        db.Planners.DeleteOnSubmit(plann);
                    }
                    break;
                default:
                    break;
            }

             //inserting to appropriate table after changing type;
            switch (type)
            {
              
                case 1:                    
                    
                    admin.UserID = type;
                    db.Admins.InsertOnSubmit(admin);
                    break;
                case 2:

                    customer.UserID = type;
                    db.Customers.InsertOnSubmit(customer);
                    break;
                case 3:

                    employee.UserID = type;
                    db.Employees.InsertOnSubmit(employee);
                    break;
                case 4:

                    planner.UserID = type;
                    db.Planners.InsertOnSubmit(planner);
                    break;
                default:


                    break;
            }

            try
            {
                db.SubmitChanges();
                response = "Success";
            }
            catch (Exception exception)
            {
                response = exception.Message;
            }
            return response;
        }



        public string getUsername(string email)
        {
            throw new NotImplementedException();
        }
    }
}
