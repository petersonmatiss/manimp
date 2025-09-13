# EN 1090 Quick Reference Guide

This document provides a quick reference for developers and project managers working with EN 1090 compliance in the Manimp application.

## Quick Decision Matrix

### Execution Class Selection
| Project Type | Risk Level | Seismic | Environment | Suggested Class |
|--------------|------------|---------|-------------|-----------------|
| Simple warehouse | Low | Low | Indoor dry | **EXC1** |
| Office building | Medium | Low-Med | Indoor/sheltered | **EXC2** |
| Hospital/school | High | Med | Any | **EXC3** |
| Bridge/tower | High | High | Marine/exposed | **EXC4** |

### Certificate Requirements by Execution Class
| Execution Class | Certificate Type | Heat Number | Chemical Analysis | Mechanical Testing |
|-----------------|------------------|-------------|-------------------|-------------------|
| **EXC1** | EN 10204 Type 2.1 | Optional | Basic | Basic |
| **EXC2** | EN 10204 Type 3.1 | Required | Standard | Standard |
| **EXC3** | EN 10204 Type 3.1 | Required | Enhanced | Enhanced |
| **EXC4** | EN 10204 Type 3.2 | Required | Full | Full + Impact |

### NDT Requirements by Execution Class
| Test Type | EXC1 | EXC2 | EXC3 | EXC4 |
|-----------|------|------|------|------|
| **Visual (VT)** | 100% | 100% | 100% | 100% |
| **Magnetic Particle (MT)** | - | Selected | Extensive | 100% |
| **Ultrasonic (UT)** | - | Critical | Comprehensive | 100% |
| **Radiographic (RT)** | - | If specified | Selected | Comprehensive |
| **Penetrant (PT)** | - | If specified | Critical areas | All accessible |

## Implementation Checklists

### Phase 1: Basic Compliance (3 months)
- [ ] Database schema for execution classes
- [ ] Execution class determination wizard
- [ ] Material certificate upload system
- [ ] Basic compliance dashboard
- [ ] Project compliance status tracking

### Phase 2: Welding & Inspection (6 months)
- [ ] WPS/WPQR management system
- [ ] Welder qualification tracking
- [ ] Inspection planning automation
- [ ] NDT scheduling and recording
- [ ] Quality document generation

### Phase 3: Advanced Features (12 months)
- [ ] Automated DoP generation
- [ ] Full traceability reporting
- [ ] Mobile inspection app
- [ ] Advanced analytics dashboard
- [ ] External system integrations

## Common API Endpoints

### Execution Class Management
```http
POST /api/projects/{projectId}/execution-class
GET /api/projects/{projectId}/execution-class
PUT /api/projects/{projectId}/execution-class/{classId}
```

### Material Certificates
```http
POST /api/inventory/{inventoryId}/certificates
GET /api/certificates/{certificateId}
POST /api/certificates/{certificateId}/verify
GET /api/inventory/{inventoryId}/traceability
```

### Compliance Status
```http
GET /api/projects/{projectId}/compliance-status
GET /api/projects/{projectId}/outstanding-requirements
POST /api/projects/{projectId}/declaration-of-performance
```

## Database Quick Reference

### Key Tables
- `ExecutionClasses` - Project execution class determinations
- `MaterialCertificates` - Material certificate storage and verification
- `WeldingProcedures` - WPS/WPQR library
- `WelderQualifications` - Welder certification tracking
- `InspectionActivities` - Inspection planning and results
- `ComplianceRequirements` - Project compliance requirements matrix

### Important Relationships
```
Projects -> ExecutionClasses -> ComplianceRequirements
ProfileInventory -> MaterialCertificates -> MaterialTestResults
Projects -> InspectionActivities -> ComplianceRequirements
WeldingProcedures -> WelderQualifications
```

## Feature Gates

### EN 1090 Features by Plan
| Feature | Basic | Professional | Enterprise |
|---------|-------|--------------|------------|
| Execution class determination | ❌ | ✅ | ✅ |
| Material certificates | ❌ | ✅ | ✅ |
| Basic compliance dashboard | ❌ | ✅ | ✅ |
| Welding management | ❌ | ❌ | ✅ |
| Advanced inspection | ❌ | ❌ | ✅ |
| Document generation | ❌ | ❌ | ✅ |

## Key Standards References

### Primary Standards
- **EN 1090-1:2009+A1:2011** - Conformity assessment requirements
- **EN 1090-2:2018** - Technical requirements for steel structures
- **EN ISO 5817:2014** - Weld quality levels
- **EN 10204:2004** - Material inspection documents

### Welding Standards
- **EN ISO 9606-1:2017** - Welder qualification testing
- **ISO 3834** - Quality requirements for welding
- **EN ISO 15614-1** - Welding procedure qualification

### NDT Standards
- **EN ISO 17638** - Magnetic particle testing
- **EN ISO 17640** - Ultrasonic testing
- **EN ISO 17636** - Radiographic testing

## Troubleshooting Common Issues

### Certificate Upload Failures
1. Check file size limits (max 10MB recommended)
2. Verify supported formats (PDF, JPG, PNG)
3. Ensure certificate metadata is complete
4. Validate heat/batch number format

### Execution Class Conflicts
1. Review project parameters for accuracy
2. Check consequence class determination
3. Verify seismic category selection
4. Consider special requirements or client specifications

### Compliance Status Discrepancies
1. Verify all required certificates are uploaded
2. Check inspection completion status
3. Review welding procedure assignments
4. Validate welder qualifications currency

## Performance Optimization Tips

### Database Queries
- Use indexes on frequently queried columns
- Implement pagination for large result sets
- Cache execution class determination results
- Optimize compliance status calculations

### File Storage
- Store certificates in Azure Blob Storage
- Implement CDN for frequently accessed documents
- Use compression for large certificate files
- Implement automatic cleanup for expired items

### User Experience
- Implement progressive loading for dashboards
- Cache compliance status on client side
- Use background processing for heavy calculations
- Provide real-time status updates via SignalR

---

*This quick reference guide is designed for daily use by development teams working on EN 1090 compliance features in the Manimp application.*

**Last Updated:** January 2025  
**Version:** 1.0