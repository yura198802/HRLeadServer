using Monica.Core.DbModel.ModelCrm.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Monica.Core.Abstraction.CriteriaCalculate
{
    public interface ICriteriaCalculate
    {
        ClientCriteria CreateCriteriaAge(int clientId);
        ClientCriteria CreateCriteriaMortgage(int clientId);
        ClientCriteria CreateCriteriaCredit(int clientId);
        ClientCriteria CreateCriteriaCreditCard(int clientId);
        ClientCriteria CreateCriteriaCommonBalance(int clientId);
        ClientCriteria CreateCriteriaSalary(int clientId);
        ClientCriteria CreateCriteriaPOS(int clientId);

    }
}
