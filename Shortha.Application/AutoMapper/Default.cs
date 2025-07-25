﻿using AutoMapper;
using Shortha.Application.AutoMapper;
using Shortha.Domain.Dto;

namespace Shortha.Application.Dto.AutoMapper;

public class Default : Profile
{

    public Default()
    {
        CreateMap(typeof(PaginationResult<>), typeof(PaginationResult<>))
            .ConvertUsing(typeof(PagedResultConverter<,>));
        
    }
    
}