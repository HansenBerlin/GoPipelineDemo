﻿using System.Text;
using Newtonsoft.Json;
using PaymentApplication.ValueObjects;
using PaymentCore.Emuns;
using PaymentCore.Entities;
using PaymentCore.Interfaces;
using PaymentCore.UseCases;

namespace PaymentApplication.Controller;

public class SavingsAccountController : HttpRequestController, ISavingsAccountInteractor
{
    public SavingsAccountController(HttpClient client, string requestBaseUrl) : base(client, requestBaseUrl) { }

    public async Task<IUserSavingsAccount> AddAccount(string username)
    {
        HttpResponseMessage response = await Client.GetAsync($"{RequestBaseUrl}/{ApiStrings.AddNewSavingsAccount}/{username.ToLower()}/0");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsAsync<SavingsAccountEntity>();
        }

        return new SavingsAccountEntity();
    }

    public async Task <PaymentState> MakePayment(IPayment paymentData, IUserSavingsAccount account)
    {
        bool areFundsInsufficient = paymentData.Amount > account.Savings + account.NegativeAllowed * -1;
        if (areFundsInsufficient)
        {
            return PaymentState.InvalidFunds;
        }
        PaymentState result = PaymentState.Pending;
        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(paymentData), Encoding.UTF8);
        httpContent.Headers.ContentType = new("application/json");
        HttpResponseMessage response = await Client.PostAsync($"{RequestBaseUrl}/{ApiStrings.MakePayment}", httpContent);
        if (response.IsSuccessStatusCode)
        {
            string responseMessage = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<PaymentState>(responseMessage);

        }
        return result;
    }

    public async Task<PaymentState> Deposit(IPayment paymentData)
    {
        PaymentState result = PaymentState.Pending;
        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(paymentData), Encoding.UTF8);
        httpContent.Headers.ContentType = new("application/json");
        HttpResponseMessage response = await Client.PostAsync($"{RequestBaseUrl}/{ApiStrings.Deposit}", httpContent);
        if (response.IsSuccessStatusCode)
        {
            string responseMessage = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<PaymentState>(responseMessage);

        }
        return result;
    }

    public async Task<bool> IsAccountAvailable(int iD)
    {
        HttpResponseMessage response = await Client.GetAsync($"{RequestBaseUrl}/{ApiStrings.CheckAccountAvailabilityById}/{iD}");
        bool isAccountAvailable = false;
        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();
            isAccountAvailable = bool.Parse(result);
        }
        return isAccountAvailable;
    }

    public async Task<IUserSavingsAccount> GetUserAccount(string username)
    {
        HttpResponseMessage response = await Client.GetAsync($"{RequestBaseUrl}/{ApiStrings.CheckAccountAvailabilityByUser}/{username}");
        IUserSavingsAccount savingsAccount = new SavingsAccountEntity();
        if (response.IsSuccessStatusCode)
        {
            savingsAccount = await response.Content.ReadAsAsync<SavingsAccountEntity>();
        }
        return savingsAccount;
    }
}