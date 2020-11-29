
using Monica.Core.Abstraction.CriteriaCalculate;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Client;
using System.Linq;
using Monica.Core.DbModel.ModelCrm.Client.Enums;
using System;


namespace Monica.Core.Service.CriteriaCalculate
{
    public class CriteriaCalculation : ICriteriaCalculate
    {
        private ClientDbContext _dbContext;
        public CriteriaCalculation(ClientDbContext dbContext )
        {
            _dbContext = dbContext;
        }
        public virtual ClientCriteria CreateCriteriaAge(int clientId)
        {
            var targetClient = _dbContext.Client.FirstOrDefault(f => f.Id == clientId);
            if (targetClient == null)
                throw new Exception("Не найден клиент с Id = " + clientId);
            ClientCriteria result = CreateCustomCriteria(clientId, "AGE", "Возраст", TypeValue.Long, targetClient.age);
            if (result == null)
                throw new Exception("Не удалось создать критэрий");
            result.Client = targetClient;
            _dbContext.SaveChanges();
            return result;
        }
        public virtual ClientCriteria CreateCriteriaMortgage(int clientId)
        {
            var targetClient = _dbContext.Client.FirstOrDefault(f => f.Id == clientId);
            if (targetClient == null)
                throw new Exception("Не найден клиент с Id = " + clientId);
            ClientCriteria result = CreateCustomCriteria(clientId, "Mortgage", "Долг по ипотеке", TypeValue.Double, targetClient.aMRG_eop);
            if (result == null)
                throw new Exception("Не удалось создать критэрий");
            result.Client = targetClient;
            _dbContext.SaveChanges();
            return result;
        }
        public virtual ClientCriteria CreateCriteriaCredit(int clientId)
        {
            var targetClient = _dbContext.Client.FirstOrDefault(f => f.Id == clientId);
            if (targetClient == null)
                throw new Exception("Не найден клиент с Id = " + clientId);
            ClientCriteria result = CreateCustomCriteria(clientId, "Credit", "Остаток долга по потреб кредиту", TypeValue.Double, targetClient.aCSH_eop);
            if (result == null)
                throw new Exception("Не удалось создать критэрий");
            result.Client = targetClient;
            _dbContext.SaveChanges();
            return result;
        }
        public virtual ClientCriteria CreateCriteriaCreditCard(int clientId)
        {
            var targetClient = _dbContext.Client.FirstOrDefault(f => f.Id == clientId);
            if (targetClient == null)
                throw new Exception("Не найден клиент с Id = " + clientId);
            ClientCriteria result = CreateCustomCriteria(clientId, "Credit", "Остаток долга по кред карте", TypeValue.Double, targetClient.aCRD_eop);
            if (result == null)
                throw new Exception("Не удалось создать критэрий");
            result.Client = targetClient;
            _dbContext.SaveChanges();
            return result;
        }
        public virtual ClientCriteria CreateCriteriaCommonBalance(int clientId)
        {
            var targetClient = _dbContext.Client.FirstOrDefault(f => f.Id == clientId);
            if (targetClient == null)
                throw new Exception("Не найден клиент с Id = " + clientId);
            var balance = targetClient.pCUR_eop + targetClient.pCRD_eop + targetClient.pSAV_eop + targetClient.pSAV_eop;
            ClientCriteria result = CreateCustomCriteria(clientId, "Balance", "Сумарный баланс клиента", TypeValue.Double, balance);
            if (result == null)
                throw new Exception("Не удалось создать критэрий");
            result.Client = targetClient;
            _dbContext.SaveChanges();
            return result;
        }
        public virtual ClientCriteria CreateCriteriaSalary(int clientId)
        {
            var targetClient = _dbContext.Client.FirstOrDefault(f => f.Id == clientId);
            if (targetClient == null)
                throw new Exception("Не найден клиент с Id = " + clientId);
            ClientCriteria result = CreateCustomCriteria(clientId, "Salary", "Зарплата", TypeValue.Double, targetClient.sWork_S);
            if (result == null)
                throw new Exception("Не удалось создать критэрий");
            result.Client = targetClient;
            _dbContext.SaveChanges();
            return result;
        }
        public virtual ClientCriteria CreateCriteriaPOS(int clientId)
        {
            var targetClient = _dbContext.Client.FirstOrDefault(f => f.Id == clientId);
            if (targetClient == null)
                throw new Exception("Не найден клиент с Id = " + clientId);
            ClientCriteria result = CreateCustomCriteria(clientId, "POS", "Сумма транзакций POS", TypeValue.Double, targetClient.tPOS_S);
            if (result == null)
                throw new Exception("Не удалось создать критэрий");
            result.Client = targetClient;
            _dbContext.SaveChanges();
            return result;
        }
        protected virtual ClientCriteria CreateCustomCriteria(int clientId, string code, string dicriptionCode, TypeValue typeValue, object inputValue )
        {
            ClientCriteria clientCriteries = new ClientCriteria();
            Criteria criteriaAge = _dbContext.Сriteria.FirstOrDefault(f => f.Code == code);
            if (criteriaAge == null)
            {
                criteriaAge = new Criteria();
                criteriaAge.Code = code;
                criteriaAge.Name = dicriptionCode;
                criteriaAge.TypeValue = typeValue;
            }
            clientCriteries.Criteria = criteriaAge;
            switch ((int)typeValue)
            {
                case 0: clientCriteries.BoolValue = (bool)inputValue;
                    break;
                case 1: clientCriteries.LongValue = (long)inputValue;
                    break;
                case 2: clientCriteries.DoubleValue = (double)inputValue;
                    break;
                case 3: clientCriteries.StringValue = inputValue.ToString();
                    break;
                case 4: clientCriteries.DateTimeValue = (DateTime)inputValue;
                    break;
                default: throw new Exception("Тип входного значения данных не существует в данном контексте");
            }           
            return clientCriteries;
        }
    }
}
