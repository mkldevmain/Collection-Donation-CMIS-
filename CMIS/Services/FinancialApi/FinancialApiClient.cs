using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace CMIS.Services.FinancialApi;

public interface IFinancialApiClient
{
    // Budgets
    Task<IReadOnlyList<BudgetReadDto>> GetBudgetsAsync(int? churchId = null, int? districtId = null, string? level = null, CancellationToken ct = default);
    Task<BudgetReadDto?> GetBudgetAsync(int id, CancellationToken ct = default);
    Task<BudgetReadDto?> CreateBudgetAsync(BudgetWriteDto dto, CancellationToken ct = default);
    Task<BudgetReadDto?> UpdateBudgetAsync(int id, BudgetWriteDto dto, CancellationToken ct = default);
    Task<bool> DeleteBudgetAsync(int id, CancellationToken ct = default);

    // Proposals
    Task<IReadOnlyList<BudgetProposalReadDto>> GetProposalsAsync(int? ministryId = null, int? churchId = null, int? districtId = null, string? status = null, string? level = null, CancellationToken ct = default);
    Task<BudgetProposalReadDto?> GetProposalAsync(int id, CancellationToken ct = default);
    Task<BudgetProposalReadDto?> CreateProposalAsync(BudgetProposalCreateDto dto, CancellationToken ct = default);
    Task<BudgetProposalReadDto?> UpdateProposalAsync(int id, BudgetProposalUpdateDto dto, CancellationToken ct = default);
    Task<BudgetProposalReadDto?> ChangeProposalStatusAsync(int id, BudgetProposalStatusDto dto, CancellationToken ct = default);
    Task<bool> DeleteProposalAsync(int id, CancellationToken ct = default);

    // Allocations
    Task<IReadOnlyList<BudgetAllocationReadDto>> GetAllocationsAsync(int? budgetId = null, CancellationToken ct = default);
    Task<BudgetAllocationReadDto?> GetAllocationAsync(int id, CancellationToken ct = default);
    Task<BudgetAllocationReadDto?> CreateAllocationAsync(BudgetAllocationWriteDto dto, CancellationToken ct = default);
    Task<BudgetAllocationReadDto?> UpdateAllocationAsync(int id, BudgetAllocationWriteDto dto, CancellationToken ct = default);
    Task<bool> DeleteAllocationAsync(int id, CancellationToken ct = default);

    // Transactions
    Task<IReadOnlyList<TransactionReadDto>> GetTransactionsAsync(int? budgetAllocationId = null, string? type = null, string? fromDate = null, string? toDate = null, CancellationToken ct = default);
    Task<TransactionReadDto?> GetTransactionAsync(int id, CancellationToken ct = default);
    Task<TransactionReadDto?> CreateTransactionAsync(TransactionWriteDto dto, CancellationToken ct = default);
    Task<TransactionReadDto?> UpdateTransactionAsync(int id, TransactionWriteDto dto, CancellationToken ct = default);
    Task<bool> DeleteTransactionAsync(int id, CancellationToken ct = default);
}

public class FinancialApiClient : IFinancialApiClient
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public FinancialApiClient(HttpClient http) { _http = http; }

    public Task<IReadOnlyList<BudgetReadDto>> GetBudgetsAsync(int? churchId = null, int? districtId = null, string? level = null, CancellationToken ct = default)
        => GetListAsync<BudgetReadDto>(Build("Budget", ("churchId", churchId), ("districtId", districtId), ("level", level)), ct);

    public Task<BudgetReadDto?> GetBudgetAsync(int id, CancellationToken ct = default)
        => GetOneAsync<BudgetReadDto>($"Budget/{id}", ct);

    public Task<BudgetReadDto?> CreateBudgetAsync(BudgetWriteDto dto, CancellationToken ct = default)
        => PostAsync<BudgetWriteDto, BudgetReadDto>("Budget", dto, ct);

    public Task<BudgetReadDto?> UpdateBudgetAsync(int id, BudgetWriteDto dto, CancellationToken ct = default)
        => PutAsync<BudgetWriteDto, BudgetReadDto>($"Budget/{id}", dto, ct);

    public Task<bool> DeleteBudgetAsync(int id, CancellationToken ct = default)
        => DeleteAsync($"Budget/{id}", ct);

    public Task<IReadOnlyList<BudgetProposalReadDto>> GetProposalsAsync(int? ministryId = null, int? churchId = null, int? districtId = null, string? status = null, string? level = null, CancellationToken ct = default)
        => GetListAsync<BudgetProposalReadDto>(Build("BudgetProposal",
            ("ministryId", ministryId), ("churchId", churchId), ("districtId", districtId), ("status", status), ("level", level)), ct);

    public Task<BudgetProposalReadDto?> GetProposalAsync(int id, CancellationToken ct = default)
        => GetOneAsync<BudgetProposalReadDto>($"BudgetProposal/{id}", ct);

    public Task<BudgetProposalReadDto?> CreateProposalAsync(BudgetProposalCreateDto dto, CancellationToken ct = default)
        => PostAsync<BudgetProposalCreateDto, BudgetProposalReadDto>("BudgetProposal", dto, ct);

    public Task<BudgetProposalReadDto?> UpdateProposalAsync(int id, BudgetProposalUpdateDto dto, CancellationToken ct = default)
        => PutAsync<BudgetProposalUpdateDto, BudgetProposalReadDto>($"BudgetProposal/{id}", dto, ct);

    public Task<BudgetProposalReadDto?> ChangeProposalStatusAsync(int id, BudgetProposalStatusDto dto, CancellationToken ct = default)
        => PatchAsync<BudgetProposalStatusDto, BudgetProposalReadDto>($"BudgetProposal/{id}/status", dto, ct);

    public Task<bool> DeleteProposalAsync(int id, CancellationToken ct = default)
        => DeleteAsync($"BudgetProposal/{id}", ct);

    public Task<IReadOnlyList<BudgetAllocationReadDto>> GetAllocationsAsync(int? budgetId = null, CancellationToken ct = default)
        => GetListAsync<BudgetAllocationReadDto>(Build("BudgetAllocation", ("budgetId", budgetId)), ct);

    public Task<BudgetAllocationReadDto?> GetAllocationAsync(int id, CancellationToken ct = default)
        => GetOneAsync<BudgetAllocationReadDto>($"BudgetAllocation/{id}", ct);

    public Task<BudgetAllocationReadDto?> CreateAllocationAsync(BudgetAllocationWriteDto dto, CancellationToken ct = default)
        => PostAsync<BudgetAllocationWriteDto, BudgetAllocationReadDto>("BudgetAllocation", dto, ct);

    public Task<BudgetAllocationReadDto?> UpdateAllocationAsync(int id, BudgetAllocationWriteDto dto, CancellationToken ct = default)
        => PutAsync<BudgetAllocationWriteDto, BudgetAllocationReadDto>($"BudgetAllocation/{id}", dto, ct);

    public Task<bool> DeleteAllocationAsync(int id, CancellationToken ct = default)
        => DeleteAsync($"BudgetAllocation/{id}", ct);

    public Task<IReadOnlyList<TransactionReadDto>> GetTransactionsAsync(int? budgetAllocationId = null, string? type = null, string? fromDate = null, string? toDate = null, CancellationToken ct = default)
        => GetListAsync<TransactionReadDto>(Build("Transaction",
            ("budgetAllocationId", budgetAllocationId), ("type", type), ("fromDate", fromDate), ("toDate", toDate)), ct);

    public Task<TransactionReadDto?> GetTransactionAsync(int id, CancellationToken ct = default)
        => GetOneAsync<TransactionReadDto>($"Transaction/{id}", ct);

    public Task<TransactionReadDto?> CreateTransactionAsync(TransactionWriteDto dto, CancellationToken ct = default)
        => PostAsync<TransactionWriteDto, TransactionReadDto>("Transaction", dto, ct);

    public Task<TransactionReadDto?> UpdateTransactionAsync(int id, TransactionWriteDto dto, CancellationToken ct = default)
        => PutAsync<TransactionWriteDto, TransactionReadDto>($"Transaction/{id}", dto, ct);

    public Task<bool> DeleteTransactionAsync(int id, CancellationToken ct = default)
        => DeleteAsync($"Transaction/{id}", ct);

    private async Task<IReadOnlyList<T>> GetListAsync<T>(string url, CancellationToken ct)
    {
        var result = await _http.GetFromJsonAsync<List<T>>(url, JsonOptions, ct);
        return result ?? new List<T>();
    }

    private async Task<T?> GetOneAsync<T>(string url, CancellationToken ct)
    {
        var response = await _http.GetAsync(url, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return default;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
    }

    private async Task<TOut?> PostAsync<TIn, TOut>(string url, TIn body, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync(url, body, JsonOptions, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TOut>(JsonOptions, ct);
    }

    private async Task<TOut?> PutAsync<TIn, TOut>(string url, TIn body, CancellationToken ct)
    {
        var response = await _http.PutAsJsonAsync(url, body, JsonOptions, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return default;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TOut>(JsonOptions, ct);
    }

    private async Task<TOut?> PatchAsync<TIn, TOut>(string url, TIn body, CancellationToken ct)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json")
        };
        var response = await _http.SendAsync(request, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return default;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TOut>(JsonOptions, ct);
    }

    private async Task<bool> DeleteAsync(string url, CancellationToken ct)
    {
        var response = await _http.DeleteAsync(url, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    private static string Build(string path, params (string Key, object? Value)[] parameters)
    {
        var query = parameters
            .Where(p => p.Value != null && !(p.Value is string s && string.IsNullOrEmpty(s)))
            .Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value!.ToString()!)}");
        var queryString = string.Join("&", query);
        return string.IsNullOrEmpty(queryString) ? path : $"{path}?{queryString}";
    }
}
