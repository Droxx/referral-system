namespace ReferralService.Core.UseCases;

public interface IUseCase<in TIn>
{
    Task Handle(TIn input, CancellationToken cancellationToken= default); 
}

public interface IUseCase<in TIn, TOut>
{
    Task<TOut> Handle(TIn input, CancellationToken cancellationToken= default);
}